using LIOSCare.ClinicPortal.Web.Models;
using LIOSCare.ClinicPortal.Web.Security;
using LIOSCare.ClinicPortal.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIOSCare.ClinicPortal.Web.Controllers;

[Authorize(Roles = PortalRoles.Admin)]
public sealed class AdminController(ICurrentUserService current, IClinicAdminService clinic, IIdentityManagementService identity) : Controller
{
    private Guid ClinicId => current.ClinicId ?? throw new UnauthorizedAccessException("Admin is not assigned to clinic/hospital.");
    public async Task<IActionResult> Dashboard(CancellationToken ct) => View(await clinic.GetAdminMetricsAsync(ClinicId, ct));
    public async Task<IActionResult> Doctors(CancellationToken ct) => View(await clinic.GetDoctorsAsync(ClinicId, ct));
    public async Task<IActionResult> Doctor(Guid? id, CancellationToken ct)
    {
        if (!id.HasValue) return View(new DoctorFormVm());
        var d = await clinic.GetDoctorAsync(ClinicId, id.Value, ct) ?? throw new KeyNotFoundException();
        return View(new DoctorFormVm { Id = d.Id, UserId = d.UserId, FullName = d.User?.FullName ?? d.DisplayName, Email = d.User?.Email ?? string.Empty, DisplayName = d.DisplayName, SpecializationsCsv = string.Join(", ", d.Specializations), YearsExperience = d.YearsExperience, LicenseNumber = d.LicenseNumber, Bio = d.Bio, IsAvailable = d.IsAvailable, MaxConcurrentSessions = d.MaxConcurrentSessions, ChatSessionPrice = d.ChatSessionPrice, OnlineConsultationPrice = d.OnlineConsultationPrice, ChatSessionMinutes = d.ChatSessionMinutes, OnlineConsultationMinutes = d.OnlineConsultationMinutes, Currency = d.Currency, WorkingDaysCsv = string.Join(", ", d.WorkingDays), WorkStartTime = d.WorkStartTime, WorkEndTime = d.WorkEndTime });
    }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Doctor(DoctorFormVm model, CancellationToken ct) { if (!ModelState.IsValid) return View(model); var result = await clinic.SaveDoctorAsync(model, ClinicId, ct); if (!result.Succeeded) { foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description); return View(model); } return RedirectToAction(nameof(Doctors)); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> SetDoctorStatus(Guid id, string status, CancellationToken ct) { await clinic.SetDoctorStatusAsync(ClinicId, id, status, ct); return RedirectToAction(nameof(Doctors)); }
    public IActionResult CreateUser() => View(new CreateUserVm { Role = PortalRoles.Admin, ClinicHospitalId = ClinicId });
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> CreateUser(CreateUserVm model, CancellationToken ct) { model.ClinicHospitalId = ClinicId; var result = await identity.CreateUserAsync(model, ct); if (!result.Succeeded) { foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description); return View(model); } return RedirectToAction(nameof(Dashboard)); }
}
