using System.ComponentModel.DataAnnotations;

namespace LIOSCare.ClinicPortal.Web.Models;

public sealed record MetricVm(string Title, string Value, string Hint, string Icon, string Tone);
public sealed record AppUserVm(Guid Id, string FullName, string Email, string Role, string Status, Guid? ClinicId);
public sealed record FacilityVm(Guid Id, string Name, string Type, string Status, string City, string Country, string ContactEmail, Guid? AdminUserId);
public sealed record DoctorVm(Guid Id, Guid UserId, string DisplayName, string Status, string LicenseNumber, string[] Specializations, decimal ChatPrice, decimal ConsultationPrice, int MaxConcurrentSessions, bool IsAvailable);
public sealed record JobVm(Guid Id, string AnonymousPatient, string JobType, string TierName, string Status, DateTimeOffset OfferExpiresAt, string Note, Guid? PreferredDoctorId);
public sealed record ChatSessionVm(Guid Id, string AnonymousPatient, string JobType, string Status, int DurationMinutes, DateTimeOffset ScheduledStartAt, DateTimeOffset AutoCloseAt, string? CloseReason);
public sealed record NotificationVm(Guid Id, string Title, string Body, string Type, DateTimeOffset CreatedAt, bool IsRead);
public sealed record RescheduleVm(Guid Id, Guid ChatSessionId, string AnonymousPatient, DateTimeOffset RequestedStartAt, string Reason, string Status);

public sealed record EmptyStateVm(string Title, string Message, string Icon = "bi-inbox", string? ActionUrl = null, string? ActionText = null, string ActionIcon = "bi-plus-lg");

public sealed class LoginVm
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

public sealed class CreateUserVm
{
    [Required, MaxLength(160)] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, MinLength(8)] public string Password { get; set; } = string.Empty;
    [Required] public string Role { get; set; } = string.Empty;
    public Guid? ClinicHospitalId { get; set; }
}

public sealed class FacilityFormVm
{
    public Guid? Id { get; set; }
    [Required, MaxLength(160)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(30)] public string Type { get; set; } = "Clinic";
    [Required, MaxLength(255)] public string ContactEmail { get; set; } = string.Empty;
    [Required, MaxLength(40)] public string ContactPhone { get; set; } = string.Empty;
    [Required, MaxLength(120)] public string City { get; set; } = string.Empty;
    [Required, MaxLength(120)] public string Country { get; set; } = string.Empty;
    [Required] public string Address { get; set; } = string.Empty;
    public Guid? AdminUserId { get; set; }
}

public sealed class DoctorFormVm
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    [Required, MaxLength(160)] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    [Required, MaxLength(140)] public string DisplayName { get; set; } = string.Empty;
    [Required] public string SpecializationsCsv { get; set; } = string.Empty;
    [Range(0, 70)] public int YearsExperience { get; set; }
    [Required, MaxLength(80)] public string LicenseNumber { get; set; } = string.Empty;
    [Required] public string Bio { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    [Range(1, 3)] public int MaxConcurrentSessions { get; set; } = 3;
    [Range(0, 10000)] public decimal ChatSessionPrice { get; set; }
    [Range(0, 10000)] public decimal OnlineConsultationPrice { get; set; }
    [Range(5, 180)] public int ChatSessionMinutes { get; set; } = 30;
    [Range(5, 240)] public int OnlineConsultationMinutes { get; set; } = 40;
    [Required, MaxLength(3)] public string Currency { get; set; } = "USD";
    public string WorkingDaysCsv { get; set; } = "Monday,Tuesday,Wednesday,Thursday,Friday";
    public TimeOnly? WorkStartTime { get; set; }
    public TimeOnly? WorkEndTime { get; set; }
}

public sealed class RejectJobVm
{
    [Required] public Guid JobId { get; set; }
    [Required, MaxLength(500)] public string Reason { get; set; } = string.Empty;
}

public sealed class SendMessageVm
{
    [Required] public Guid SessionId { get; set; }
    [Required, MaxLength(2000)] public string Body { get; set; } = string.Empty;
}

public sealed class SessionReportFormVm
{
    public Guid? Id { get; set; }
    [Required] public Guid ChatSessionId { get; set; }
    [Range(1,10)] public int MoodBefore { get; set; } = 4;
    [Range(1,10)] public int MoodAfter { get; set; } = 7;
    [Range(0,100)] public int GoalsCompleted { get; set; }
    [Range(1,100)] public int GoalsTotal { get; set; } = 1;
    [Required, MaxLength(2000)] public string ProgressNotes { get; set; } = string.Empty;
    public string TechniquesUsedCsv { get; set; } = string.Empty;
    [MaxLength(500)] public string? NextSteps { get; set; }
}

public sealed class ProfileVm
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastLoginAt { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}

public sealed class EditProfileVm
{
    [Required, MaxLength(160)] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
}

public sealed class ChangePasswordVm
{
    [Required, DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(8)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
    [Display(Name = "Confirm New Password")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
