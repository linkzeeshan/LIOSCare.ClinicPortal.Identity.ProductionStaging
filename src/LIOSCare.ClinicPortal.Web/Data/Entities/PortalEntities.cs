using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIOSCare.ClinicPortal.Web.Data.Entities;

public sealed class ClinicHospital
{
    public Guid Id { get; set; }
    [MaxLength(160)] public string Name { get; set; } = string.Empty;
    [MaxLength(30)] public string Type { get; set; } = "Clinic";
    [MaxLength(30)] public string Status { get; set; } = "PendingApproval";
    [MaxLength(255)] public string ContactEmail { get; set; } = string.Empty;
    [MaxLength(40)] public string ContactPhone { get; set; } = string.Empty;
    [MaxLength(120)] public string City { get; set; } = string.Empty;
    [MaxLength(120)] public string Country { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid? AdminUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class DoctorProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public Guid? ClinicId { get; set; }
    public ClinicHospital? Clinic { get; set; }
    [MaxLength(140)] public string DisplayName { get; set; } = string.Empty;
    public string[] Specializations { get; set; } = [];
    public int YearsExperience { get; set; }
    [MaxLength(80)] public string LicenseNumber { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public int MaxConcurrentSessions { get; set; } = 3;
    [MaxLength(30)] public string Status { get; set; } = "PendingApproval";
    [Column(TypeName = "numeric(18,2)")] public decimal ChatSessionPrice { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal OnlineConsultationPrice { get; set; }
    public int ChatSessionMinutes { get; set; } = 30;
    public int OnlineConsultationMinutes { get; set; } = 40;
    [MaxLength(3)] public string Currency { get; set; } = "USD";
    public string[] WorkingDays { get; set; } = [];
    public TimeOnly? WorkStartTime { get; set; }
    public TimeOnly? WorkEndTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class PatientAccount
{
    public Guid Id { get; set; }
    [MaxLength(80)] public string AnonymousCode { get; set; } = string.Empty;
    [MaxLength(120)] public string AnonymousDisplayName { get; set; } = string.Empty;
    [MaxLength(255)] public string? UserPortalEmail { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class ServiceTier
{
    public Guid Id { get; set; }
    [MaxLength(40)] public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    [Column(TypeName = "numeric(18,2)")] public decimal Price { get; set; }
    [MaxLength(3)] public string Currency { get; set; } = "USD";
    [MaxLength(30)] public string ServiceType { get; set; } = "Chat";
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public sealed class ChatSessionJob
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public PatientAccount? Patient { get; set; }
    public Guid? PreferredDoctorId { get; set; }
    public Guid? ClinicId { get; set; }
    public Guid ServiceTierId { get; set; }
    public ServiceTier? ServiceTier { get; set; }
    [MaxLength(30)] public string JobType { get; set; } = "QuickChat";
    [MaxLength(30)] public string Status { get; set; } = "Open";
    public string PatientNotePublic { get; set; } = string.Empty;
    public DateTimeOffset OfferExpiresAt { get; set; }
    public Guid? AcceptedByDoctorId { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public DateTimeOffset? RejectedAt { get; set; }
    public string? RejectedReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class ChatSession
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public ChatSessionJob? Job { get; set; }
    public Guid DoctorId { get; set; }
    public DoctorProfile? Doctor { get; set; }
    public Guid PatientId { get; set; }
    public PatientAccount? Patient { get; set; }
    public Guid ServiceTierId { get; set; }
    public ServiceTier? ServiceTier { get; set; }
    [MaxLength(30)] public string Status { get; set; } = "Active";
    public int DurationMinutes { get; set; }
    public DateTimeOffset ScheduledStartAt { get; set; }
    public DateTimeOffset? ActualStartedAt { get; set; }
    public DateTimeOffset AutoCloseAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public string? CloseReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class ChatMessage
{
    public Guid Id { get; set; }
    public Guid ChatSessionId { get; set; }
    public ChatSession? ChatSession { get; set; }
    [MaxLength(20)] public string SenderType { get; set; } = "Doctor";
    public Guid? SenderUserId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class RescheduleRequest
{
    public Guid Id { get; set; }
    public Guid ChatSessionId { get; set; }
    public ChatSession? ChatSession { get; set; }
    public Guid PatientId { get; set; }
    public PatientAccount? Patient { get; set; }
    public DateTimeOffset RequestedStartAt { get; set; }
    public string Reason { get; set; } = string.Empty;
    [MaxLength(30)] public string Status { get; set; } = "Pending";
    public Guid? RespondedByDoctorId { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class NotificationItem
{
    public Guid Id { get; set; }
    public Guid RecipientUserId { get; set; }
    [MaxLength(120)] public string Type { get; set; } = string.Empty;
    [MaxLength(180)] public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    [MaxLength(80)] public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class SessionReport
{
    public Guid Id { get; set; }
    public Guid ChatSessionId { get; set; }
    public ChatSession? ChatSession { get; set; }
    public Guid DoctorId { get; set; }
    public DoctorProfile? Doctor { get; set; }
    [MaxLength(120)] public string AnonymousPatientLabel { get; set; } = string.Empty;
    public DateOnly SessionDate { get; set; }
    public int MoodBefore { get; set; }
    public int MoodAfter { get; set; }
    public int GoalsCompleted { get; set; }
    public int GoalsTotal { get; set; }
    public string ProgressNotes { get; set; } = string.Empty;
    public string[] TechniquesUsed { get; set; } = [];
    public string? NextSteps { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LockedAt { get; set; }
}
