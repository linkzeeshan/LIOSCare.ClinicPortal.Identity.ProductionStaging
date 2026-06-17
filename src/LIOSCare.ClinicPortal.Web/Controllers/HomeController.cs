using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LIOSCare.ClinicPortal.Web.Controllers;

[AllowAnonymous]
public sealed class HomeController : Controller
{
    public IActionResult Error() => View();
}
