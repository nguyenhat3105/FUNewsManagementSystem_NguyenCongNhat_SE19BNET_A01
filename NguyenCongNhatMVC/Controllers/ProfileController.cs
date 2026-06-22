using Microsoft.AspNetCore.Mvc;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Services;

namespace NguyenCongNhatMVC.Controllers;

public class ProfileController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IWebHostEnvironment _environment;

    public ProfileController(IAccountService accountService, IWebHostEnvironment environment)
    {
        _accountService = accountService;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0)
        {
            return RedirectToAction("Index", "News");
        }

        var account = await _accountService.GetAsync(user.AccountID);
        return account == null ? NotFound() : View(account);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SystemAccount account, string? newPassword, IFormFile? avatarFile)
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0 || user.AccountID != account.AccountID)
        {
            return Forbid();
        }

        var existing = await _accountService.GetAsync(user.AccountID);
        if (existing == null)
        {
            return NotFound();
        }

        account.AccountRole = existing.AccountRole;
        if (!ModelState.IsValid)
        {
            return View(account);
        }

        if (avatarFile != null && avatarFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var extension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(account.AvatarUrl), "Please choose a JPG, PNG, WEBP, or GIF image.");
                return View(account);
            }

            if (avatarFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(account.AvatarUrl), "Avatar image must be 2 MB or smaller.");
                return View(account);
            }

            var avatarDirectory = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(avatarDirectory);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(avatarDirectory, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await avatarFile.CopyToAsync(stream);

            account.AvatarUrl = $"/uploads/avatars/{fileName}";
        }

        existing.AccountName = account.AccountName;
        existing.AccountEmail = account.AccountEmail;
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            existing.AccountPassword = newPassword.Trim();
        }
        existing.PhoneNumber = account.PhoneNumber;
        existing.AvatarUrl = account.AvatarUrl;
        existing.Bio = account.Bio;

        await _accountService.SaveAsync(existing);
        user.Name = existing.AccountName;
        user.Email = existing.AccountEmail;
        user.AvatarUrl = existing.AvatarUrl;
        HttpContext.Session.SetUser(user);
        TempData["Success"] = "Profile updated.";
        return RedirectToAction(nameof(Index));
    }
}
