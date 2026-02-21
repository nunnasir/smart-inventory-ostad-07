using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.BLL.Helpers;
using SmartInventory.Contract.Request.Account;
using SmartInventory.Model;

namespace SmartInventory.Web.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if(!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, 
            model.Password, 
            model.RememberMe, false);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Login successfully!";
            return RedirectToLocal(returnUrl);
        }

        TempData["ErrorMessage"] = "Invalid login attempt.";
        return View(model);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var (isValidEmail, emailError) = EmailValidator.Validate(model?.Email);
        if (!isValidEmail)
        {
            ModelState.AddModelError(nameof(RegisterModel.Email), emailError ?? "Invalid email format.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if(result.Succeeded)
        {
            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["SuccessMessage"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Index()
    {
        return View();
    }

}
