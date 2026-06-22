using LIOSCare.ClinicPortal.Web.Models;
using LIOSCare.ClinicPortal.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LIOSCare.ClinicPortal.Web.Controllers;

[Authorize]
[Route("profile")]
public sealed class ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();
        
        var model = new ProfileVm
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.UpdatedAt,
            Roles = await userManager.GetRolesAsync(user)
        };
        
        return View(model);
    }

    [HttpGet("edit")]
    public async Task<IActionResult> Edit()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();
        
        var model = new EditProfileVm
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty
        };
        
        return View(model);
    }

    [HttpPost("edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileVm model)
    {
        if (!ModelState.IsValid) return View(model);
        
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();
        
        user.FullName = model.FullName;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        
        TempData["Success"] = "Profile updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("change-password")]
    public IActionResult ChangePassword() => View();
    
    [HttpPost("change-password"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordVm model)
    {
        if (!ModelState.IsValid) return View(model);
        
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();
        
        var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        
        await signInManager.RefreshSignInAsync(user);
        TempData["Success"] = "Password changed successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("logout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}
