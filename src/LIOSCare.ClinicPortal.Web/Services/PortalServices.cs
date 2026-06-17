using LIOSCare.ClinicPortal.Web.Data;
using LIOSCare.ClinicPortal.Web.Data.Entities;
using LIOSCare.ClinicPortal.Web.Models;
using LIOSCare.ClinicPortal.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace LIOSCare.ClinicPortal.Web.Services;

public sealed class PortalRulesOptions
{
    public int MaxActiveChatSessionsPerDoctor { get; set; } = 3;
    public int MaxActiveOnlineConsultationsPerDoctor { get; set; } = 1;
    public int AutoCloseWorkerIntervalSeconds { get; set; } = 60;
}

public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid? ClinicId { get; }
    bool IsSuperAdmin { get; }
    bool IsAdmin { get; }
    bool IsDoctor { get; }
}

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private ClaimsPrincipal User => accessor.HttpContext?.User ?? new ClaimsPrincipal();
    public Guid UserId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
    public Guid? ClinicId => Guid.TryParse(User.FindFirstValue(PortalClaims.ClinicId), out var id) ? id : null;
    public bool IsSuperAdmin => User.IsInRole(PortalRoles.SuperAdmin);
    public bool IsAdmin => User.IsInRole(PortalRoles.Admin);
    public bool IsDoctor => User.IsInRole(PortalRoles.Doctor);
}

public interface IIdentityManagementService
{
    Task<IReadOnlyList<AppUserVm>> GetSuperAdminsAsync(CancellationToken ct);
    Task<IdentityResult> CreateUserAsync(CreateUserVm model, CancellationToken ct);
    Task<IdentityResult> DeactivateUserAsync(Guid userId, CancellationToken ct);
}

public sealed class IdentityManagementService(UserManager<ApplicationUser> users, RoleManager<ApplicationRole> roles, ApplicationDbContext db, ICurrentUserService current) : IIdentityManagementService
{
    public async Task<IReadOnlyList<AppUserVm>> GetSuperAdminsAsync(CancellationToken ct)
    {
        var userIds = await db.UserRoles.Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, Role = r.Name })
            .Where(x => x.Role == PortalRoles.SuperAdmin).Select(x => x.UserId).ToListAsync(ct);
        return await db.Users.Where(x => userIds.Contains(x.Id)).OrderBy(x => x.FullName)
            .Select(x => new AppUserVm(x.Id, x.FullName, x.Email ?? string.Empty, PortalRoles.SuperAdmin, x.Status, x.ClinicHospitalId)).ToListAsync(ct);
    }

    public async Task<IdentityResult> CreateUserAsync(CreateUserVm model, CancellationToken ct)
    {
        if (!current.IsSuperAdmin && current.IsAdmin)
        {
            model.ClinicHospitalId = current.ClinicId;
            if (model.Role == PortalRoles.SuperAdmin) return IdentityResult.Failed(new IdentityError { Description = "Clinic Admin cannot create Super Admin users." });
        }
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(), UserName = model.Email, NormalizedUserName = model.Email.ToUpperInvariant(),
            Email = model.Email, NormalizedEmail = model.Email.ToUpperInvariant(), FullName = model.FullName,
            EmailConfirmed = true, Status = "Active", ClinicHospitalId = model.ClinicHospitalId,
            SecurityStamp = Guid.NewGuid().ToString("N"), ConcurrencyStamp = Guid.NewGuid().ToString("N")
        };
        var result = await users.CreateAsync(user, model.Password);
        if (!result.Succeeded) return result;
        if (!await roles.RoleExistsAsync(model.Role)) await roles.CreateAsync(new ApplicationRole { Name = model.Role, Description = model.Role });
        result = await users.AddToRoleAsync(user, model.Role);
        if (!result.Succeeded) return result;
        if (user.ClinicHospitalId.HasValue) await users.AddClaimAsync(user, new Claim(PortalClaims.ClinicId, user.ClinicHospitalId.Value.ToString()));
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeactivateUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(userId.ToString());
        if (user is null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        if (current.IsAdmin && user.ClinicHospitalId != current.ClinicId) return IdentityResult.Failed(new IdentityError { Description = "Tenant isolation violation." });
        user.Status = "Inactive"; user.DeactivatedAt = DateTimeOffset.UtcNow; user.UpdatedAt = DateTimeOffset.UtcNow;
        return await users.UpdateAsync(user);
    }
}

public interface IFacilityService
{
    Task<IReadOnlyList<MetricVm>> GetSuperAdminMetricsAsync(CancellationToken ct);
    Task<IReadOnlyList<FacilityVm>> GetFacilitiesAsync(CancellationToken ct);
    Task<ClinicHospital?> GetFacilityAsync(Guid id, CancellationToken ct);
    Task SaveFacilityAsync(FacilityFormVm model, CancellationToken ct);
    Task SetFacilityStatusAsync(Guid id, string status, CancellationToken ct);
}

public sealed class FacilityService(ApplicationDbContext db) : IFacilityService
{
    public async Task<IReadOnlyList<MetricVm>> GetSuperAdminMetricsAsync(CancellationToken ct)
    {
        var facilities = await db.ClinicsHospitals.CountAsync(ct);
        var pending = await db.ClinicsHospitals.CountAsync(x => x.Status == "PendingApproval", ct);
        var doctors = await db.DoctorProfiles.CountAsync(ct);
        var jobs = await db.ChatSessionJobs.CountAsync(x => x.Status == "Open", ct);
        return [new("Facilities", facilities.ToString(), "clinics/hospitals", "bi-buildings", "blue"), new("Pending Approval", pending.ToString(), "waiting review", "bi-shield-check", "amber"), new("Doctors", doctors.ToString(), "registered", "bi-person-vcard", "green"), new("Open Jobs", jobs.ToString(), "mobile bids", "bi-broadcast", "violet")];
    }
    public async Task<IReadOnlyList<FacilityVm>> GetFacilitiesAsync(CancellationToken ct) => await db.ClinicsHospitals.OrderByDescending(x => x.CreatedAt).Select(x => new FacilityVm(x.Id, x.Name, x.Type, x.Status, x.City, x.Country, x.ContactEmail, x.AdminUserId)).ToListAsync(ct);
    public Task<ClinicHospital?> GetFacilityAsync(Guid id, CancellationToken ct) => db.ClinicsHospitals.FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task SaveFacilityAsync(FacilityFormVm model, CancellationToken ct)
    {
        var facility = model.Id.HasValue ? await db.ClinicsHospitals.FirstAsync(x => x.Id == model.Id.Value, ct) : new ClinicHospital { Id = Guid.NewGuid(), Status = "PendingApproval" };
        facility.Name = model.Name; facility.Type = model.Type; facility.ContactEmail = model.ContactEmail; facility.ContactPhone = model.ContactPhone; facility.City = model.City; facility.Country = model.Country; facility.Address = model.Address; facility.AdminUserId = model.AdminUserId; facility.UpdatedAt = DateTimeOffset.UtcNow;
        if (!model.Id.HasValue) db.ClinicsHospitals.Add(facility);
        await db.SaveChangesAsync(ct);
    }
    public async Task SetFacilityStatusAsync(Guid id, string status, CancellationToken ct)
    {
        var facility = await db.ClinicsHospitals.FirstAsync(x => x.Id == id, ct);
        facility.Status = status; facility.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
    }
}

public interface IClinicAdminService
{
    Task<IReadOnlyList<MetricVm>> GetAdminMetricsAsync(Guid clinicId, CancellationToken ct);
    Task<IReadOnlyList<DoctorVm>> GetDoctorsAsync(Guid clinicId, CancellationToken ct);
    Task<DoctorProfile?> GetDoctorAsync(Guid clinicId, Guid id, CancellationToken ct);
    Task<IdentityResult> SaveDoctorAsync(DoctorFormVm model, Guid clinicId, CancellationToken ct);
    Task SetDoctorStatusAsync(Guid clinicId, Guid doctorId, string status, CancellationToken ct);
}

public sealed class ClinicAdminService(ApplicationDbContext db, UserManager<ApplicationUser> users, RoleManager<ApplicationRole> roles) : IClinicAdminService
{
    public async Task<IReadOnlyList<MetricVm>> GetAdminMetricsAsync(Guid clinicId, CancellationToken ct)
    {
        var doctors = await db.DoctorProfiles.CountAsync(x => x.ClinicId == clinicId, ct);
        var active = await db.DoctorProfiles.CountAsync(x => x.ClinicId == clinicId && x.Status == "Active", ct);
        var sessions = await db.ChatSessions.CountAsync(x => x.Doctor!.ClinicId == clinicId && x.Status == "Active", ct);
        var jobs = await db.ChatSessionJobs.CountAsync(x => x.ClinicId == clinicId && x.Status == "Open", ct);
        return [new("Doctors", doctors.ToString(), "clinic roster", "bi-person-vcard", "blue"), new("Active Doctors", active.ToString(), "approved", "bi-check2-circle", "green"), new("Live Sessions", sessions.ToString(), "currently open", "bi-chat-dots", "violet"), new("Open Jobs", jobs.ToString(), "waiting acceptance", "bi-bell", "amber")];
    }
    public async Task<IReadOnlyList<DoctorVm>> GetDoctorsAsync(Guid clinicId, CancellationToken ct) => await db.DoctorProfiles.Where(x => x.ClinicId == clinicId).OrderBy(x => x.DisplayName).Select(x => new DoctorVm(x.Id, x.UserId, x.DisplayName, x.Status, x.LicenseNumber, x.Specializations, x.ChatSessionPrice, x.OnlineConsultationPrice, x.MaxConcurrentSessions, x.IsAvailable)).ToListAsync(ct);
    public Task<DoctorProfile?> GetDoctorAsync(Guid clinicId, Guid id, CancellationToken ct) => db.DoctorProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id && x.ClinicId == clinicId, ct);
    public async Task<IdentityResult> SaveDoctorAsync(DoctorFormVm model, Guid clinicId, CancellationToken ct)
    {
        ApplicationUser user;
        if (model.UserId.HasValue)
        {
            user = await users.FindByIdAsync(model.UserId.Value.ToString()) ?? throw new InvalidOperationException("Doctor user not found.");
            if (user.ClinicHospitalId != clinicId) return IdentityResult.Failed(new IdentityError { Description = "Tenant isolation violation." });
            user.FullName = model.FullName; user.Email = model.Email; user.UserName = model.Email; user.NormalizedEmail = model.Email.ToUpperInvariant(); user.NormalizedUserName = model.Email.ToUpperInvariant();
            await users.UpdateAsync(user);
        }
        else
        {
            user = new ApplicationUser { Id = Guid.NewGuid(), FullName = model.FullName, Email = model.Email, UserName = model.Email, NormalizedEmail = model.Email.ToUpperInvariant(), NormalizedUserName = model.Email.ToUpperInvariant(), ClinicHospitalId = clinicId, EmailConfirmed = true, Status = "Active", SecurityStamp = Guid.NewGuid().ToString("N"), ConcurrencyStamp = Guid.NewGuid().ToString("N") };
            var create = await users.CreateAsync(user, string.IsNullOrWhiteSpace(model.Password) ? "Doctor@123" : model.Password);
            if (!create.Succeeded) return create;
            if (!await roles.RoleExistsAsync(PortalRoles.Doctor)) await roles.CreateAsync(new ApplicationRole { Name = PortalRoles.Doctor });
            await users.AddToRoleAsync(user, PortalRoles.Doctor);
            await users.AddClaimAsync(user, new Claim(PortalClaims.ClinicId, clinicId.ToString()));
        }
        var profile = model.Id.HasValue ? await db.DoctorProfiles.FirstAsync(x => x.Id == model.Id.Value && x.ClinicId == clinicId, ct) : new DoctorProfile { Id = Guid.NewGuid(), UserId = user.Id, ClinicId = clinicId, Status = "PendingApproval" };
        profile.DisplayName = model.DisplayName; profile.Specializations = Csv(model.SpecializationsCsv); profile.YearsExperience = model.YearsExperience; profile.LicenseNumber = model.LicenseNumber; profile.Bio = model.Bio; profile.IsAvailable = model.IsAvailable; profile.MaxConcurrentSessions = Math.Clamp(model.MaxConcurrentSessions, 1, 3); profile.ChatSessionPrice = model.ChatSessionPrice; profile.OnlineConsultationPrice = model.OnlineConsultationPrice; profile.ChatSessionMinutes = model.ChatSessionMinutes; profile.OnlineConsultationMinutes = model.OnlineConsultationMinutes; profile.Currency = model.Currency; profile.WorkingDays = Csv(model.WorkingDaysCsv); profile.WorkStartTime = model.WorkStartTime; profile.WorkEndTime = model.WorkEndTime; profile.UpdatedAt = DateTimeOffset.UtcNow;
        if (!model.Id.HasValue) db.DoctorProfiles.Add(profile);
        await db.SaveChangesAsync(ct);
        return IdentityResult.Success;
    }
    public async Task SetDoctorStatusAsync(Guid clinicId, Guid doctorId, string status, CancellationToken ct)
    {
        var doctor = await db.DoctorProfiles.FirstAsync(x => x.Id == doctorId && x.ClinicId == clinicId, ct);
        doctor.Status = status; doctor.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
    }
    private static string[] Csv(string value) => value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
}

public interface IDoctorWorkspaceService
{
    Task<DoctorProfile> GetMyDoctorProfileAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<MetricVm>> GetDoctorMetricsAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<JobVm>> GetOpenJobsAsync(Guid userId, CancellationToken ct);
    Task AcceptJobAsync(Guid userId, Guid jobId, CancellationToken ct);
    Task RejectJobAsync(Guid userId, RejectJobVm model, CancellationToken ct);
    Task<IReadOnlyList<ChatSessionVm>> GetSessionsAsync(Guid userId, bool history, CancellationToken ct);
    Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(Guid userId, Guid sessionId, CancellationToken ct);
    Task SendMessageAsync(Guid userId, SendMessageVm model, CancellationToken ct);
    Task<IReadOnlyList<RescheduleVm>> GetRescheduleRequestsAsync(Guid userId, CancellationToken ct);
    Task RespondRescheduleAsync(Guid userId, Guid requestId, bool approve, CancellationToken ct);
    Task<IReadOnlyList<NotificationVm>> GetNotificationsAsync(Guid userId, CancellationToken ct);
    Task SaveReportAsync(Guid userId, SessionReportFormVm model, CancellationToken ct);
}

public sealed class DoctorWorkspaceService(ApplicationDbContext db, IOptions<PortalRulesOptions> rules) : IDoctorWorkspaceService
{
    public async Task<DoctorProfile> GetMyDoctorProfileAsync(Guid userId, CancellationToken ct) => await db.DoctorProfiles.FirstAsync(x => x.UserId == userId, ct);
    public async Task<IReadOnlyList<MetricVm>> GetDoctorMetricsAsync(Guid userId, CancellationToken ct)
    {
        var doc = await GetMyDoctorProfileAsync(userId, ct);
        var active = await db.ChatSessions.CountAsync(x => x.DoctorId == doc.Id && x.Status == "Active", ct);
        var jobs = await db.ChatSessionJobs.CountAsync(x => x.Status == "Open" && x.OfferExpiresAt > DateTimeOffset.UtcNow && (x.PreferredDoctorId == null || x.PreferredDoctorId == doc.Id) && (x.ClinicId == null || x.ClinicId == doc.ClinicId), ct);
        var history = await db.ChatSessions.CountAsync(x => x.DoctorId == doc.Id && x.Status == "Closed", ct);
        var notifications = await db.Notifications.CountAsync(x => x.RecipientUserId == userId && x.ReadAt == null, ct);
        return [new("Open Jobs", jobs.ToString(), "available offers", "bi-broadcast", "amber"), new("Active Sessions", active.ToString(), "max 3 chat / 1 online", "bi-chat-dots", "blue"), new("History", history.ToString(), "completed sessions", "bi-clock-history", "green"), new("Notifications", notifications.ToString(), "unread", "bi-bell", "violet")];
    }
    public async Task<IReadOnlyList<JobVm>> GetOpenJobsAsync(Guid userId, CancellationToken ct)
    {
        var doc = await GetMyDoctorProfileAsync(userId, ct);
        return await db.ChatSessionJobs.Include(x => x.Patient).Include(x => x.ServiceTier).Where(x => x.Status == "Open" && x.OfferExpiresAt > DateTimeOffset.UtcNow && (x.PreferredDoctorId == null || x.PreferredDoctorId == doc.Id) && (x.ClinicId == null || x.ClinicId == doc.ClinicId)).OrderBy(x => x.OfferExpiresAt).Select(x => new JobVm(x.Id, x.Patient!.AnonymousDisplayName, x.JobType, x.ServiceTier!.Name, x.Status, x.OfferExpiresAt, x.PatientNotePublic, x.PreferredDoctorId)).ToListAsync(ct);
    }
    public async Task AcceptJobAsync(Guid userId, Guid jobId, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        var doctor = await GetMyDoctorProfileAsync(userId, ct);
        var job = await db.ChatSessionJobs.Include(x => x.ServiceTier).FirstAsync(x => x.Id == jobId, ct);
        if (job.Status != "Open" || job.OfferExpiresAt <= DateTimeOffset.UtcNow) throw new InvalidOperationException("This job is no longer available.");
        if (job.ClinicId.HasValue && doctor.ClinicId != job.ClinicId) throw new UnauthorizedAccessException("This job belongs to another clinic/hospital.");
        if (job.PreferredDoctorId.HasValue && job.PreferredDoctorId != doctor.Id) throw new UnauthorizedAccessException("This job was assigned to another doctor.");
        var activeChat = await db.ChatSessions.CountAsync(x => x.DoctorId == doctor.Id && x.Status == "Active" && x.Job!.JobType != "OnlineConsultation", ct);
        var activeOnline = await db.ChatSessions.CountAsync(x => x.DoctorId == doctor.Id && x.Status == "Active" && x.Job!.JobType == "OnlineConsultation", ct);
        if (job.JobType == "OnlineConsultation" && activeOnline >= rules.Value.MaxActiveOnlineConsultationsPerDoctor) throw new InvalidOperationException("Only one online consultation can be active at the same time.");
        if (job.JobType != "OnlineConsultation" && activeChat >= rules.Value.MaxActiveChatSessionsPerDoctor) throw new InvalidOperationException("Maximum 3 active chat sessions are allowed at the same time.");
        var now = DateTimeOffset.UtcNow;
        job.Status = "Accepted"; job.AcceptedByDoctorId = doctor.Id; job.AcceptedAt = now; job.UpdatedAt = now;
        var duration = job.ServiceTier?.DurationMinutes ?? (job.JobType == "OnlineConsultation" ? doctor.OnlineConsultationMinutes : doctor.ChatSessionMinutes);
        var session = new ChatSession { Id = Guid.NewGuid(), JobId = job.Id, DoctorId = doctor.Id, PatientId = job.PatientId, ServiceTierId = job.ServiceTierId, Status = "Active", DurationMinutes = duration, ScheduledStartAt = now, ActualStartedAt = now, AutoCloseAt = now.AddMinutes(duration), CreatedAt = now, UpdatedAt = now };
        db.ChatSessions.Add(session);
        db.Notifications.Add(new NotificationItem { Id = Guid.NewGuid(), RecipientUserId = userId, Type = "SessionStarted", Title = "Session started", Body = "A new anonymous patient session has started.", EntityType = "ChatSession", EntityId = session.Id });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }
    public async Task RejectJobAsync(Guid userId, RejectJobVm model, CancellationToken ct)
    {
        var doc = await GetMyDoctorProfileAsync(userId, ct);
        var job = await db.ChatSessionJobs.FirstAsync(x => x.Id == model.JobId && (x.PreferredDoctorId == null || x.PreferredDoctorId == doc.Id), ct);
        job.Status = "Rejected"; job.RejectedAt = DateTimeOffset.UtcNow; job.RejectedReason = model.Reason; job.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<ChatSessionVm>> GetSessionsAsync(Guid userId, bool history, CancellationToken ct)
    {
        var doc = await GetMyDoctorProfileAsync(userId, ct);
        var query = db.ChatSessions.Include(x => x.Patient).Include(x => x.Job).Where(x => x.DoctorId == doc.Id);
        query = history ? query.Where(x => x.Status == "Closed") : query.Where(x => x.Status != "Closed");
        return await query.OrderByDescending(x => x.CreatedAt).Select(x => new ChatSessionVm(x.Id, x.Patient!.AnonymousDisplayName, x.Job!.JobType, x.Status, x.DurationMinutes, x.ScheduledStartAt, x.AutoCloseAt, x.CloseReason)).ToListAsync(ct);
    }
    public async Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(Guid userId, Guid sessionId, CancellationToken ct)
    {
        var doc = await GetMyDoctorProfileAsync(userId, ct);
        var owns = await db.ChatSessions.AnyAsync(x => x.Id == sessionId && x.DoctorId == doc.Id, ct);
        if (!owns) throw new UnauthorizedAccessException();
        return await db.ChatMessages.Where(x => x.ChatSessionId == sessionId).OrderBy(x => x.CreatedAt).ToListAsync(ct);
    }
    public async Task SendMessageAsync(Guid userId, SendMessageVm model, CancellationToken ct)
    {
        var doc = await GetMyDoctorProfileAsync(userId, ct);
        var session = await db.ChatSessions.FirstAsync(x => x.Id == model.SessionId && x.DoctorId == doc.Id, ct);
        if (session.Status != "Active" || session.AutoCloseAt <= DateTimeOffset.UtcNow) throw new InvalidOperationException("This session is closed.");
        db.ChatMessages.Add(new ChatMessage { Id = Guid.NewGuid(), ChatSessionId = session.Id, SenderType = "Doctor", SenderUserId = userId, Body = model.Body });
        await db.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<RescheduleVm>> GetRescheduleRequestsAsync(Guid userId, CancellationToken ct)
    {
        var doc = await GetMyDoctorProfileAsync(userId, ct);
        return await db.RescheduleRequests.Include(x => x.Patient).Include(x => x.ChatSession).Where(x => x.ChatSession!.DoctorId == doc.Id && x.Status == "Pending").Select(x => new RescheduleVm(x.Id, x.ChatSessionId, x.Patient!.AnonymousDisplayName, x.RequestedStartAt, x.Reason, x.Status)).ToListAsync(ct);
    }
    public async Task RespondRescheduleAsync(Guid userId, Guid requestId, bool approve, CancellationToken ct)
    {
        var doc = await GetMyDoctorProfileAsync(userId, ct);
        var req = await db.RescheduleRequests.Include(x => x.ChatSession).FirstAsync(x => x.Id == requestId && x.ChatSession!.DoctorId == doc.Id, ct);
        req.Status = approve ? "Approved" : "Rejected"; req.RespondedByDoctorId = doc.Id; req.RespondedAt = DateTimeOffset.UtcNow;
        if (approve) { req.ChatSession!.ScheduledStartAt = req.RequestedStartAt; req.ChatSession.AutoCloseAt = req.RequestedStartAt.AddMinutes(req.ChatSession.DurationMinutes); req.ChatSession.UpdatedAt = DateTimeOffset.UtcNow; }
        await db.SaveChangesAsync(ct);
    }
    public async Task<IReadOnlyList<NotificationVm>> GetNotificationsAsync(Guid userId, CancellationToken ct) => await db.Notifications.Where(x => x.RecipientUserId == userId).OrderByDescending(x => x.CreatedAt).Take(30).Select(x => new NotificationVm(x.Id, x.Title, x.Body, x.Type, x.CreatedAt, x.ReadAt != null)).ToListAsync(ct);
    public async Task SaveReportAsync(Guid userId, SessionReportFormVm model, CancellationToken ct)
    {
        var doc = await GetMyDoctorProfileAsync(userId, ct);
        var session = await db.ChatSessions.Include(x => x.Patient).FirstAsync(x => x.Id == model.ChatSessionId && x.DoctorId == doc.Id, ct);
        db.SessionReports.Add(new SessionReport { Id = Guid.NewGuid(), ChatSessionId = session.Id, DoctorId = doc.Id, AnonymousPatientLabel = session.Patient!.AnonymousDisplayName, SessionDate = DateOnly.FromDateTime(DateTime.UtcNow), MoodBefore = model.MoodBefore, MoodAfter = model.MoodAfter, GoalsCompleted = model.GoalsCompleted, GoalsTotal = model.GoalsTotal, ProgressNotes = model.ProgressNotes, TechniquesUsed = model.TechniquesUsedCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), NextSteps = model.NextSteps, LockedAt = DateTimeOffset.UtcNow.AddHours(24) });
        await db.SaveChangesAsync(ct);
    }
}

public sealed class ChatSessionAutoCloseWorker(IServiceScopeFactory scopeFactory, IOptions<PortalRulesOptions> options, ILogger<ChatSessionAutoCloseWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = TimeSpan.FromSeconds(Math.Max(15, options.Value.AutoCloseWorkerIntervalSeconds));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var now = DateTimeOffset.UtcNow;
                var sessions = await db.ChatSessions.Where(x => x.Status == "Active" && x.AutoCloseAt <= now).ToListAsync(stoppingToken);
                foreach (var s in sessions) { s.Status = "Closed"; s.ClosedAt = now; s.CloseReason = "Auto closed after booked duration"; s.UpdatedAt = now; }
                if (sessions.Count > 0) await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex) { logger.LogError(ex, "Failed to auto-close chat sessions."); }
            await Task.Delay(delay, stoppingToken);
        }
    }
}
