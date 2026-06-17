using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LIOSCare.ClinicPortal.Web.Data.Entities;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    [MaxLength(160)] public string FullName { get; set; } = string.Empty;
    [MaxLength(30)] public string Status { get; set; } = "Active";
    public Guid? ClinicHospitalId { get; set; }
    public ClinicHospital? ClinicHospital { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeactivatedAt { get; set; }
}

public sealed class ApplicationRole : IdentityRole<Guid>
{
    [MaxLength(255)] public string? Description { get; set; }
}
