using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolHub.Mvc.Data;
using VolHub.Mvc.Models;
using VolHub.Mvc.Models.ViewModels;
using VolHub.Mvc.Security;

namespace VolHub.Mvc.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly AppDbContext _db;

    public AccountController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = "/")
    {
        var redirectUri = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUri },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        var redirectUri = Url.Action("Index", "Home") ?? "/";
        return SignOut(
            new AuthenticationProperties { RedirectUri = redirectUri },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var email = GetUserEmail();
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction(nameof(Login));
        }

        var profile = await _db.Profiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email);

        if (profile is null)
        {
            return NotFound();
        }

        var vm = new ProfileEditViewModel
        {
            Username = profile.Username,
            Email = profile.Email,
            FullName = profile.FullName,
            Phone = profile.Phone
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = GetUserEmail();
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction(nameof(Login));
        }

        var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.Email == email);
        if (profile is null)
        {
            return NotFound();
        }

        profile.FullName = model.FullName.Trim();
        profile.Phone = model.Phone.Trim();

        await _db.SaveChangesAsync();
        TempData["StatusMessage"] = "Профіль успішно оновлено.";

        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Username = model.Username.Trim();
        model.FullName = model.FullName.Trim();
        model.Email = model.Email.Trim();
        model.Phone = model.Phone.Trim();

        if (await _db.Profiles.AnyAsync(p => p.Username == model.Username))
        {
            ModelState.AddModelError(nameof(model.Username), "Такий логін уже зайнятий.");
            return View(model);
        }

        if (await _db.Profiles.AnyAsync(p => p.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Обліковий запис з таким e-mail вже існує.");
            return View(model);
        }

        var profile = new AppUserProfile
        {
            Username = model.Username,
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            PasswordHash = PasswordHelper.HashPassword(model.Password)
        };

        _db.Profiles.Add(profile);
        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = "Реєстрація успішна. Увійдіть через Auth0.";
        return RedirectToAction(nameof(Login));
    }

    private string? GetUserEmail()
        => User.FindFirstValue(ClaimTypes.Email)
           ?? User.FindFirst("email")?.Value;
}
