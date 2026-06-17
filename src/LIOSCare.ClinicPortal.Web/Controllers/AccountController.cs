using LIOSCare.ClinicPortal.Web.Data.Entities;
using LIOSCare.ClinicPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LIOSCare.ClinicPortal.Web.Controllers;

[Route("account")]
public sealed class AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : Controller
{
    [AllowAnonymous, HttpGet("login")]
    public IActionResult Login(string? returnUrl = null) { ViewData["ReturnUrl"] = returnUrl; return View(new LoginVm()); }

    [AllowAnonymous, HttpPost("login"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user is null || user.Status != "Active") { ModelState.AddModelError(string.Empty, "Invalid or inactive account."); return View(model); }
        var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (!result.Succeeded) { ModelState.AddModelError(string.Empty, "Invalid login attempt."); return View(model); }
        user.UpdatedAt = DateTimeOffset.UtcNow; await userManager.UpdateAsync(user);
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains("SuperAdmin")) return RedirectToAction("Dashboard", "SuperAdmin");
        if (roles.Contains("Admin")) return RedirectToAction("Dashboard", "Admin");
        return RedirectToAction("Dashboard", "Doctor");
    }

    [Authorize, HttpPost("logout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout() { await signInManager.SignOutAsync(); return RedirectToAction(nameof(Login)); }

    [AllowAnonymous, HttpGet("access-denied")]
    public IActionResult AccessDenied() => View();
}
