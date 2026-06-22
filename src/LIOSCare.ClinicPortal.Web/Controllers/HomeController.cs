using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIOSCare.ClinicPortal.Web.Controllers;

[AllowAnonymous]
public sealed class HomeController : Controller
{
    public IActionResult Error(string? message = null)
    {
        if (!string.IsNullOrEmpty(message))
        {
            ViewBag.ErrorMessage = Uri.UnescapeDataString(message);
        }
        return View();
    }
}
