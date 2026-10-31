using E_learning.Models;
using E_learning.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace E_learning.Controllers;

public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly RoleManager<IdentityRole> _roles;

    public UserController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, RoleManager<IdentityRole> roles)
    {
        _users = users;
        _signIn = signIn;
        _roles = roles;
    }

    [AllowAnonymous]
    public IActionResult Register() => User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Course") : View();

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken, EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new ApplicationUser
        {
            UserName = model.Email.Trim(), Email = model.Email.Trim(),
            FirstName = model.FirstName.Trim(), LastName = model.LastName.Trim(),
            Department = model.Department?.Trim(), CreatedUtc = DateTime.UtcNow
        };
        var result = await _users.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        if (!await _roles.RoleExistsAsync("Student"))
        {
            var roleResult = await _roles.CreateAsync(new IdentityRole("Student"));
            if (!roleResult.Succeeded)
            {
                await _users.DeleteAsync(user);
                ModelState.AddModelError(string.Empty, "Account setup failed. Please try again.");
                return View(model);
            }
        }
        var assigned = await _users.AddToRoleAsync(user, "Student");
        if (!assigned.Succeeded)
        {
            await _users.DeleteAsync(user);
            ModelState.AddModelError(string.Empty, "Account setup failed. Please try again.");
            return View(model);
        }
        await _signIn.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Course");
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Course") : View();
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken, EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);
        var user = await _users.FindByEmailAsync(model.Email.Trim());
        if (user is { IsActive: true })
        {
            var result = await _signIn.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: true);
            if (result.Succeeded)
                return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction("Index", "Course");
            if (result.IsLockedOut) ModelState.AddModelError(string.Empty, "Sign-in is temporarily unavailable. Try again later.");
        }
        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
