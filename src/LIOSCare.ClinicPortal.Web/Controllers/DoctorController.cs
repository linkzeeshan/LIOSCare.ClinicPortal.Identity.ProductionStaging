using LIOSCare.ClinicPortal.Web.Exceptions;
using LIOSCare.ClinicPortal.Web.Models;
using LIOSCare.ClinicPortal.Web.Security;
using LIOSCare.ClinicPortal.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIOSCare.ClinicPortal.Web.Controllers;

[Authorize(Roles = PortalRoles.Doctor)]
public sealed class DoctorController(ICurrentUserService current, IDoctorWorkspaceService doctor) : Controller
{
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        try
        {
            return View(await doctor.GetDoctorMetricsAsync(current.UserId, ct));
        }
        catch (DoctorProfileNotFoundException)
        {
            ViewBag.ErrorMessage = "Your doctor profile is not configured. Please contact your administrator to set up your profile.";
            return View("Error");
        }
    }
    public async Task<IActionResult> Jobs(CancellationToken ct) => View(await doctor.GetOpenJobsAsync(current.UserId, ct));
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> AcceptJob(Guid id, CancellationToken ct) { await doctor.AcceptJobAsync(current.UserId, id, ct); return RedirectToAction(nameof(Sessions)); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> RejectJob(RejectJobVm model, CancellationToken ct) { await doctor.RejectJobAsync(current.UserId, model, ct); return RedirectToAction(nameof(Jobs)); }
    public async Task<IActionResult> Sessions(CancellationToken ct) => View(await doctor.GetSessionsAsync(current.UserId, history: false, ct));
    public async Task<IActionResult> History(CancellationToken ct) => View("Sessions", await doctor.GetSessionsAsync(current.UserId, history: true, ct));
    public async Task<IActionResult> Session(Guid id, CancellationToken ct) { ViewBag.Messages = await doctor.GetMessagesAsync(current.UserId, id, ct); ViewBag.SessionId = id; return View(); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> SendMessage(SendMessageVm model, CancellationToken ct) { await doctor.SendMessageAsync(current.UserId, model, ct); return RedirectToAction(nameof(Session), new { id = model.SessionId }); }
    public async Task<IActionResult> Reschedules(CancellationToken ct) => View(await doctor.GetRescheduleRequestsAsync(current.UserId, ct));
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> RespondReschedule(Guid id, bool approve, CancellationToken ct) { await doctor.RespondRescheduleAsync(current.UserId, id, approve, ct); return RedirectToAction(nameof(Reschedules)); }
    public async Task<IActionResult> Notifications(CancellationToken ct) => View(await doctor.GetNotificationsAsync(current.UserId, ct));
    public IActionResult NewReport(Guid sessionId) => View(new SessionReportFormVm { ChatSessionId = sessionId });
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> NewReport(SessionReportFormVm model, CancellationToken ct) { if (!ModelState.IsValid) return View(model); await doctor.SaveReportAsync(current.UserId, model, ct); return RedirectToAction(nameof(History)); }
}
