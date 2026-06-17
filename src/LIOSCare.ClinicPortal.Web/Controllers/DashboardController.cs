using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIOSCare.ClinicPortal.Web.Controllers;

[Authorize]
public sealed class DashboardController : Controller
{
    public IActionResult Index()
    {
        if (User.IsInRole("SuperAdmin")) return RedirectToAction("Dashboard", "SuperAdmin");
        if (User.IsInRole("Admin")) return RedirectToAction("Dashboard", "Admin");
        return RedirectToAction("Dashboard", "Doctor");
    }
}
