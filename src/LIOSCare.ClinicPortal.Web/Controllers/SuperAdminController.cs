using LIOSCare.ClinicPortal.Web.Models;
using LIOSCare.ClinicPortal.Web.Security;
using LIOSCare.ClinicPortal.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIOSCare.ClinicPortal.Web.Controllers;

[Authorize(Roles = PortalRoles.SuperAdmin)]
public sealed class SuperAdminController(IFacilityService facilities, IIdentityManagementService identity) : Controller
{
    public async Task<IActionResult> Dashboard(CancellationToken ct) => View(await facilities.GetSuperAdminMetricsAsync(ct));
    public async Task<IActionResult> Facilities(CancellationToken ct) => View(await facilities.GetFacilitiesAsync(ct));
    public async Task<IActionResult> Facility(Guid? id, CancellationToken ct) => View(id.HasValue ? await facilities.GetFacilityAsync(id.Value, ct) : null);
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Facility(FacilityFormVm model, CancellationToken ct) { if (!ModelState.IsValid) return View(); await facilities.SaveFacilityAsync(model, ct); return RedirectToAction(nameof(Facilities)); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> SetFacilityStatus(Guid id, string status, CancellationToken ct) { await facilities.SetFacilityStatusAsync(id, status, ct); return RedirectToAction(nameof(Facilities)); }
    public async Task<IActionResult> SuperAdmins(CancellationToken ct) => View(await identity.GetSuperAdminsAsync(ct));
    public IActionResult CreateSuperAdmin() => View(new CreateUserVm { Role = PortalRoles.SuperAdmin });
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> CreateSuperAdmin(CreateUserVm model, CancellationToken ct) { model.Role = PortalRoles.SuperAdmin; var result = await identity.CreateUserAsync(model, ct); if (!result.Succeeded) { foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description); return View(model); } return RedirectToAction(nameof(SuperAdmins)); }
}
