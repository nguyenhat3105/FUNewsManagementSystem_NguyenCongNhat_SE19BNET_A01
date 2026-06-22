using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NguyenCongNhatMVC.Models.ViewModels;
using NguyenCongNhatMVC.Services;

namespace NguyenCongNhatMVC.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IEmailVerificationService _emailVerificationService;
    private readonly AdminAccountOptions _adminAccount;

    public AuthController(
        IAuthService authService,
        IEmailVerificationService emailVerificationService,
        IOptions<AdminAccountOptions> adminAccount)
    {
        _authService              = authService;
        _emailVerificationService = emailVerificationService;
        _adminAccount             = adminAccount.Value;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetUser() != null)
            return RedirectToAction("Index", "News");

        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _authService.LoginAsync(model.Email, model.Password);
        if (user == null)
        {
            var hint = model.Email.Trim().Equals(_adminAccount.Email, StringComparison.OrdinalIgnoreCase)
                ? "Admin password is incorrect."
                : "Invalid email or password.";
            ModelState.AddModelError(string.Empty, hint);
            return View(model);
        }

        HttpContext.Session.SetUser(user);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "News");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetUser() != null)
            return RedirectToAction("Index", "News");

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _authService.EmailExistsAsync(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email),
                "This email address is already registered. Please sign in instead.");
            return View(model);
        }

        var result = await _emailVerificationService.StartAsync(model);
        TempData["Success"] = result.EmailSent
            ? $"A verification code has been sent to {result.Email}."
            : "Email sending is disabled for local demo. Use the verification code below.";
        if (!result.EmailSent)
        {
            TempData["DemoEmailCode"] = result.Code;
        }

        return RedirectToAction(nameof(VerifyEmail));
    }

    [HttpGet]
    public IActionResult VerifyEmail()
    {
        if (HttpContext.Session.GetUser() != null)
            return RedirectToAction("Index", "News");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyEmail(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            ModelState.AddModelError(nameof(code), "Please enter the verification code.");
            return View();
        }

        if (!_emailVerificationService.TryComplete(code, out var pendingModel, out var error) || pendingModel == null)
        {
            ModelState.AddModelError(nameof(code), error ?? "Verification failed.");
            return View();
        }

        if (await _authService.EmailExistsAsync(pendingModel.Email))
        {
            _emailVerificationService.Clear();
            ModelState.AddModelError(nameof(code), "This email address is already registered.");
            return View();
        }

        var user = await _authService.RegisterAsync(pendingModel);
        if (user == null)
        {
            _emailVerificationService.Clear();
            ModelState.AddModelError(nameof(code), "Registration failed. Please register again.");
            return View();
        }

        _emailVerificationService.Clear();
        HttpContext.Session.SetUser(user);
        TempData["Success"] = $"Welcome to FUNews, {user.Name}! Your email has been verified.";
        return RedirectToAction("Index", "News");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
}
