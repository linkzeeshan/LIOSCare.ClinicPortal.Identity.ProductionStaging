using LIOSCare.ClinicPortal.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LIOSCare.ClinicPortal.Web.Data.Seeding;

public sealed class DbSeeder(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ILogger<DbSeeder> logger)
{
    public async Task SeedAsync(bool force = false, CancellationToken ct = default)
    {
        logger.LogInformation("Starting demo data seeding...");

        await EnsureRolesAsync(ct);
        await EnsureClinicAsync(ct);
        await EnsureUsersAsync(ct);
        await EnsureServiceTiersAsync(ct);
        await EnsurePatientsAsync(ct);
        await EnsureDoctorProfilesAsync(ct);
        await EnsureJobsAsync(ct);
        await EnsureSessionsAsync(ct);
        await EnsureMessagesAsync(ct);
        await EnsureNotificationsAsync(ct);
        await EnsureReschedulesAsync(ct);
        await EnsureSessionReportsAsync(ct);

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Demo data seeding completed.");
    }

    private static readonly Guid SuperAdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid AdminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid DoctorRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly Guid SuperAdminUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid AdminUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid DoctorUser1Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid DoctorUser2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    private static readonly Guid ClinicId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    private static readonly Guid ServiceTierChatId = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff1");
    private static readonly Guid ServiceTierConsultId = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff2");

    private static readonly Guid Patient1Id = Guid.Parse("11111111-1111-1111-1111-111111111112");
    private static readonly Guid Patient2Id = Guid.Parse("11111111-1111-1111-1111-111111111113");
    private static readonly Guid Patient3Id = Guid.Parse("11111111-1111-1111-1111-111111111114");

    private static readonly Guid DoctorProfile1Id = Guid.Parse("22222222-2222-2222-2222-222222222223");
    private static readonly Guid DoctorProfile2Id = Guid.Parse("22222222-2222-2222-2222-222222222224");

    private static readonly Guid Job1Id = Guid.Parse("33333333-3333-3333-3333-333333333334");
    private static readonly Guid Job2Id = Guid.Parse("33333333-3333-3333-3333-333333333335");
    private static readonly Guid Job3Id = Guid.Parse("33333333-3333-3333-3333-333333333336");
    private static readonly Guid Job4Id = Guid.Parse("33333333-3333-3333-3333-333333333337");

    private static readonly Guid Session1Id = Guid.Parse("44444444-4444-4444-4444-444444444445");
    private static readonly Guid Session2Id = Guid.Parse("44444444-4444-4444-4444-444444444446");

    private static readonly Guid Reschedule1Id = Guid.Parse("55555555-5555-5555-5555-555555555556");
    private static readonly Guid Report1Id = Guid.Parse("66666666-6666-6666-6666-666666666667");

    private async Task EnsureRolesAsync(CancellationToken ct)
    {
        var roles = new[]
        {
            (SuperAdminRoleId, "SuperAdmin", "System-wide super administrator"),
            (AdminRoleId, "Admin", "Clinic or hospital administrator"),
            (DoctorRoleId, "Doctor", "Doctor who accepts jobs and chats with patients")
        };

        foreach (var (id, name, description) in roles)
        {
            if (!await roleManager.RoleExistsAsync(name))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Id = id,
                    Name = name,
                    NormalizedName = name.ToUpperInvariant(),
                    Description = description
                });
                logger.LogInformation("Created role {RoleName}.", name);
            }
        }
    }

    private async Task EnsureClinicAsync(CancellationToken ct)
    {
        if (!await db.ClinicsHospitals.AnyAsync(x => x.Id == ClinicId, ct))
        {
            db.ClinicsHospitals.Add(new ClinicHospital
            {
                Id = ClinicId,
                Name = "LIOSCare Demo Clinic",
                Type = "Clinic",
                Status = "Active",
                ContactEmail = "admin@lioscare.local",
                ContactPhone = "+1-555-0100",
                City = "New York",
                Country = "USA",
                Address = "123 Healthcare Ave, Suite 100",
                AdminUserId = AdminUserId,
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-3),
                UpdatedAt = DateTimeOffset.UtcNow
            });
            logger.LogInformation("Created demo clinic.");
        }
    }

    private async Task EnsureUsersAsync(CancellationToken ct)
    {
        var users = new[]
        {
            (SuperAdminUserId, "superadmin@lioscare.local", "Super Admin", "SuperAdmin@123", "SuperAdmin"),
            (AdminUserId, "clinicadmin@lioscare.local", "Clinic Admin", "Admin@123", "Admin"),
            (DoctorUser1Id, "doctor@lioscare.local", "Dr. Julia Adams", "Doctor@123", "Doctor"),
            (DoctorUser2Id, "doctor2@lioscare.local", "Dr. Mark Chen", "Doctor@123", "Doctor")
        };

        foreach (var (id, email, fullName, password, role) in users)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is null)
            {
                var user = new ApplicationUser
                {
                    Id = id,
                    Email = email,
                    UserName = email,
                    FullName = fullName,
                    Status = "Active",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    CreatedAt = DateTimeOffset.UtcNow.AddMonths(-2),
                    UpdatedAt = DateTimeOffset.UtcNow,
                    ClinicHospitalId = role == "Doctor" ? ClinicId : null
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to create user {Email}: {Errors}", email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    continue;
                }

                await userManager.AddToRoleAsync(user, role);
                logger.LogInformation("Created demo user {Email} with role {Role}.", email, role);
            }
        }
    }

    private async Task EnsureServiceTiersAsync(CancellationToken ct)
    {
        if (!await db.ServiceTiers.AnyAsync(x => x.Id == ServiceTierChatId, ct))
        {
            db.ServiceTiers.Add(new ServiceTier
            {
                Id = ServiceTierChatId,
                Name = "Quick Chat",
                DurationMinutes = 30,
                Price = 25.00m,
                Currency = "USD",
                ServiceType = "Chat",
                IsActive = true,
                SortOrder = 1
            });
        }

        if (!await db.ServiceTiers.AnyAsync(x => x.Id == ServiceTierConsultId, ct))
        {
            db.ServiceTiers.Add(new ServiceTier
            {
                Id = ServiceTierConsultId,
                Name = "Online Consultation",
                DurationMinutes = 40,
                Price = 50.00m,
                Currency = "USD",
                ServiceType = "Consultation",
                IsActive = true,
                SortOrder = 2
            });
        }
    }

    private async Task EnsurePatientsAsync(CancellationToken ct)
    {
        var patients = new[]
        {
            (Patient1Id, "P-10001", "Patient A"),
            (Patient2Id, "P-10002", "Patient B"),
            (Patient3Id, "P-10003", "Patient C")
        };

        foreach (var (id, code, name) in patients)
        {
            if (!await db.PatientAccounts.AnyAsync(x => x.Id == id, ct))
            {
                db.PatientAccounts.Add(new PatientAccount
                {
                    Id = id,
                    AnonymousCode = code,
                    AnonymousDisplayName = name,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-7)
                });
            }
        }
    }

    private async Task EnsureDoctorProfilesAsync(CancellationToken ct)
    {
        var profiles = new[]
        {
            (DoctorProfile1Id, DoctorUser1Id, "Dr. Julia Adams", "LIC-MD-001", new[] { "General Practice", "Mental Health" }, 30, 60),
            (DoctorProfile2Id, DoctorUser2Id, "Dr. Mark Chen", "LIC-MD-002", new[] { "Pediatrics", "Nutrition" }, 35, 70)
        };

        foreach (var (id, userId, displayName, license, specs, chatPrice, consultPrice) in profiles)
        {
            if (!await db.DoctorProfiles.AnyAsync(x => x.Id == id, ct))
            {
                db.DoctorProfiles.Add(new DoctorProfile
                {
                    Id = id,
                    UserId = userId,
                    ClinicId = ClinicId,
                    DisplayName = displayName,
                    Specializations = specs,
                    YearsExperience = 8,
                    LicenseNumber = license,
                    Bio = $"{displayName} is a board-certified physician with over 8 years of clinical experience.",
                    IsAvailable = true,
                    MaxConcurrentSessions = 3,
                    Status = "Approved",
                    ChatSessionPrice = chatPrice,
                    OnlineConsultationPrice = consultPrice,
                    ChatSessionMinutes = 30,
                    OnlineConsultationMinutes = 40,
                    Currency = "USD",
                    WorkingDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" },
                    WorkStartTime = new TimeOnly(9, 0),
                    WorkEndTime = new TimeOnly(17, 0),
                    CreatedAt = DateTimeOffset.UtcNow.AddMonths(-2),
                    UpdatedAt = DateTimeOffset.UtcNow
                });
                logger.LogInformation("Created doctor profile for {DisplayName}.", displayName);
            }
        }
    }

    private async Task EnsureJobsAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var jobs = new[]
        {
            (Job1Id, Patient1Id, ServiceTierChatId, "QuickChat", "Open", "Patient feels mild anxiety and would like quick guidance.", now.AddHours(2)),
            (Job2Id, Patient2Id, ServiceTierConsultId, "Consultation", "Open", "Requesting nutrition consultation for a child.", now.AddHours(4)),
            (Job3Id, Patient3Id, ServiceTierChatId, "QuickChat", "Accepted", "Follow-up chat regarding previous advice.", now.AddHours(1)),
            (Job4Id, Patient1Id, ServiceTierConsultId, "Consultation", "Open", "Detailed consultation about sleep hygiene.", now.AddHours(6))
        };

        foreach (var (id, patientId, tierId, jobType, status, note, expires) in jobs)
        {
            if (!await db.ChatSessionJobs.AnyAsync(x => x.Id == id, ct))
            {
                db.ChatSessionJobs.Add(new ChatSessionJob
                {
                    Id = id,
                    PatientId = patientId,
                    ServiceTierId = tierId,
                    JobType = jobType,
                    Status = status,
                    PatientNotePublic = note,
                    OfferExpiresAt = expires,
                    AcceptedByDoctorId = status == "Accepted" ? DoctorProfile1Id : null,
                    AcceptedAt = status == "Accepted" ? now.AddMinutes(-30) : null,
                    CreatedAt = now.AddHours(-2),
                    UpdatedAt = now
                });
            }
        }
    }

    private async Task EnsureSessionsAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        if (!await db.ChatSessions.AnyAsync(x => x.Id == Session1Id, ct))
        {
            db.ChatSessions.Add(new ChatSession
            {
                Id = Session1Id,
                JobId = Job3Id,
                DoctorId = DoctorProfile1Id,
                PatientId = Patient3Id,
                ServiceTierId = ServiceTierChatId,
                Status = "Active",
                DurationMinutes = 30,
                ScheduledStartAt = now.AddMinutes(-15),
                ActualStartedAt = now.AddMinutes(-15),
                AutoCloseAt = now.AddMinutes(15),
                CreatedAt = now.AddMinutes(-20),
                UpdatedAt = now
            });
        }

        if (!await db.ChatSessions.AnyAsync(x => x.Id == Session2Id, ct))
        {
            db.ChatSessions.Add(new ChatSession
            {
                Id = Session2Id,
                JobId = Job1Id,
                DoctorId = DoctorProfile1Id,
                PatientId = Patient1Id,
                ServiceTierId = ServiceTierChatId,
                Status = "Scheduled",
                DurationMinutes = 30,
                ScheduledStartAt = now.AddHours(2),
                AutoCloseAt = now.AddHours(2).AddMinutes(30),
                CreatedAt = now.AddHours(-1),
                UpdatedAt = now
            });
        }
    }

    private async Task EnsureMessagesAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        if (!await db.ChatMessages.AnyAsync(x => x.ChatSessionId == Session1Id, ct))
        {
            db.ChatMessages.AddRange(
                new ChatMessage { ChatSessionId = Session1Id, SenderType = "Patient", Body = "Hello doctor, I wanted to follow up on my anxiety symptoms.", CreatedAt = now.AddMinutes(-14) },
                new ChatMessage { ChatSessionId = Session1Id, SenderType = "Doctor", SenderUserId = DoctorUser1Id, Body = "Of course. How have you been feeling since our last chat?", CreatedAt = now.AddMinutes(-13) },
                new ChatMessage { ChatSessionId = Session1Id, SenderType = "Patient", Body = "Much better, but I still have trouble sleeping some nights.", CreatedAt = now.AddMinutes(-10) },
                new ChatMessage { ChatSessionId = Session1Id, SenderType = "Doctor", SenderUserId = DoctorUser1Id, Body = "That is progress. Let's continue with the sleep hygiene plan.", CreatedAt = now.AddMinutes(-8) }
            );
        }
    }

    private async Task EnsureNotificationsAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        if (!await db.Notifications.AnyAsync(x => x.RecipientUserId == DoctorUser1Id, ct))
        {
            db.Notifications.AddRange(
                new NotificationItem { RecipientUserId = DoctorUser1Id, Type = "JobOffer", Title = "New job offer", Body = "A new QuickChat job is available.", EntityType = "ChatSessionJob", EntityId = Job1Id, CreatedAt = now.AddHours(-1) },
                new NotificationItem { RecipientUserId = DoctorUser1Id, Type = "JobOffer", Title = "New job offer", Body = "A new Consultation job is available.", EntityType = "ChatSessionJob", EntityId = Job2Id, CreatedAt = now.AddHours(-2) },
                new NotificationItem { RecipientUserId = DoctorUser1Id, Type = "Reschedule", Title = "Reschedule request", Body = "Patient C requested a new session time.", EntityType = "RescheduleRequest", EntityId = Reschedule1Id, CreatedAt = now.AddMinutes(-30) }
            );
        }
    }

    private async Task EnsureReschedulesAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        if (!await db.RescheduleRequests.AnyAsync(x => x.Id == Reschedule1Id, ct))
        {
            db.RescheduleRequests.Add(new RescheduleRequest
            {
                Id = Reschedule1Id,
                ChatSessionId = Session2Id,
                PatientId = Patient1Id,
                RequestedStartAt = now.AddHours(3),
                Reason = "I have a conflict at the scheduled time. Can we move it one hour later?",
                Status = "Pending",
                CreatedAt = now.AddMinutes(-30)
            });
        }
    }

    private async Task EnsureSessionReportsAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        if (!await db.SessionReports.AnyAsync(x => x.Id == Report1Id, ct))
        {
            db.SessionReports.Add(new SessionReport
            {
                Id = Report1Id,
                ChatSessionId = Session1Id,
                DoctorId = DoctorProfile1Id,
                AnonymousPatientLabel = "Patient C",
                SessionDate = DateOnly.FromDateTime(now.Date),
                MoodBefore = 4,
                MoodAfter = 7,
                GoalsCompleted = 2,
                GoalsTotal = 3,
                ProgressNotes = "Patient reports improved anxiety management. Sleep issues remain but are less frequent.",
                TechniquesUsed = new[] { "Breathing exercises", "Sleep hygiene checklist" },
                NextSteps = "Continue current plan and review in one week.",
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1),
                LockedAt = now.AddDays(-1)
            });
        }
    }
}
