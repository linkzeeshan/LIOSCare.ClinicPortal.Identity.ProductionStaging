using LIOSCare.ClinicPortal.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LIOSCare.ClinicPortal.Web.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<ClinicHospital> ClinicsHospitals => Set<ClinicHospital>();
    public DbSet<DoctorProfile> DoctorProfiles => Set<DoctorProfile>();
    public DbSet<PatientAccount> PatientAccounts => Set<PatientAccount>();
    public DbSet<ServiceTier> ServiceTiers => Set<ServiceTier>();
    public DbSet<ChatSessionJob> ChatSessionJobs => Set<ChatSessionJob>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<RescheduleRequest> RescheduleRequests => Set<RescheduleRequest>();
    public DbSet<NotificationItem> Notifications => Set<NotificationItem>();
    public DbSet<SessionReport> SessionReports => Set<SessionReport>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        ConfigureModel(builder);
    }

    internal static void ConfigureModel(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.Property(x => x.FullName).HasMaxLength(160).HasDefaultValue("");
            entity.Property(x => x.Status).HasMaxLength(30).HasDefaultValue("Active");
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
            entity.HasOne(x => x.ClinicHospital).WithMany().HasForeignKey(x => x.ClinicHospitalId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.ClinicHospitalId);
        });
        builder.Entity<ApplicationRole>().ToTable("AspNetRoles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("AspNetUserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("AspNetUserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("AspNetUserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AspNetRoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("AspNetUserTokens");

        builder.Entity<ClinicHospital>(entity =>
        {
            entity.ToTable("clinics_hospitals", "portal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.AdminUserId);
        });

        builder.Entity<DoctorProfile>(entity =>
        {
            entity.ToTable("doctor_profiles", "portal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.Specializations).HasColumnType("text[]");
            entity.Property(x => x.WorkingDays).HasColumnType("text[]");
            entity.Property(x => x.ChatSessionPrice).HasColumnType("numeric(18,2)");
            entity.Property(x => x.OnlineConsultationPrice).HasColumnType("numeric(18,2)");
            entity.Property(x => x.WorkStartTime).HasColumnType("time without time zone");
            entity.Property(x => x.WorkEndTime).HasColumnType("time without time zone");
            entity.HasIndex(x => new { x.ClinicId, x.Status });
            entity.HasIndex(x => x.UserId).IsUnique();
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Clinic).WithMany().HasForeignKey(x => x.ClinicId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PatientAccount>(entity =>
        {
            entity.ToTable("patient_accounts", "portal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.HasIndex(x => x.AnonymousCode).IsUnique();
        });

        builder.Entity<ServiceTier>(entity =>
        {
            entity.ToTable("service_tiers", "portal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.Price).HasColumnType("numeric(18,2)");
            entity.HasIndex(x => new { x.IsActive, x.SortOrder });
        });

        builder.Entity<ChatSessionJob>(entity =>
        {
            entity.ToTable("chat_session_jobs", "portal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.HasIndex(x => new { x.ClinicId, x.Status, x.OfferExpiresAt });
            entity.HasIndex(x => new { x.PreferredDoctorId, x.Status });
            entity.HasOne(x => x.Patient).WithMany().HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ServiceTier).WithMany().HasForeignKey(x => x.ServiceTierId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ChatSession>(entity =>
        {
            entity.ToTable("chat_sessions", "portal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.HasIndex(x => new { x.DoctorId, x.Status, x.AutoCloseAt });
            entity.HasIndex(x => x.PatientId);
            entity.HasOne(x => x.Job).WithMany().HasForeignKey(x => x.JobId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Doctor).WithMany().HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Patient).WithMany().HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ServiceTier).WithMany().HasForeignKey(x => x.ServiceTierId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("chat_messages", "portal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.HasIndex(x => new { x.ChatSessionId, x.CreatedAt });
            entity.HasOne(x => x.ChatSession).WithMany().HasForeignKey(x => x.ChatSessionId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RescheduleRequest>(entity =>
        {
            entity.ToTable("reschedule_requests", "portal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.HasIndex(x => new { x.ChatSessionId, x.Status });
        });

        builder.Entity<NotificationItem>(entity =>
        {
            entity.ToTable("notifications", "portal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.HasIndex(x => new { x.RecipientUserId, x.ReadAt, x.CreatedAt });
        });

        builder.Entity<SessionReport>(entity =>
        {
            entity.ToTable("session_reports", "portal");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.TechniquesUsed).HasColumnType("text[]");
            entity.HasIndex(x => new { x.DoctorId, x.SessionDate });
        });
        }
}
