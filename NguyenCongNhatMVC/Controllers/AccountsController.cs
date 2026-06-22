using Microsoft.AspNetCore.Mvc;
using System.Text;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Models.ViewModels;
using NguyenCongNhatMVC.Services;

namespace NguyenCongNhatMVC.Controllers;

public class AccountsController : Controller
{
    private readonly IAccountService _accountService;
    private readonly INewsService _newsService;
    private readonly IAuditService _auditService;

    public AccountsController(IAccountService accountService, INewsService newsService, IAuditService auditService)
    {
        _accountService = accountService;
        _newsService = newsService;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index(string? keyword)
    {
        if (!HttpContext.Session.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "News");
        }

        ViewBag.Keyword = keyword;
        return View(await _accountService.SearchAsync(keyword));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(AccountViewModel model)
    {
        if (!HttpContext.Session.IsInRole("Admin"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please check all account fields.";
            return RedirectToAction(nameof(Index));
        }

        var action = model.AccountID == 0 ? "Create" : "Update";
        await _accountService.SaveAsync(new SystemAccount
        {
            AccountID = model.AccountID,
            AccountName = model.AccountName,
            AccountEmail = model.AccountEmail,
            AccountRole = model.AccountRole,
            AccountPassword = model.AccountPassword
        });
        await _auditService.LogAsync(action, "SystemAccount", model.AccountID.ToString(), model.AccountEmail);
        TempData["Success"] = "Account saved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!HttpContext.Session.IsInRole("Admin"))
        {
            return Forbid();
        }

        if (await _accountService.DeleteAsync(id))
        {
            await _auditService.LogAsync("Delete", "SystemAccount", id.ToString());
            TempData["Success"] = "Account deleted.";
        }
        else
        {
            TempData["Error"] = "Cannot delete an account that is attached to news articles.";
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
    {
        if (!HttpContext.Session.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "News");
        }

        return View(new ReportViewModel
        {
            StartDate = startDate,
            EndDate = endDate,
            Articles = await _newsService.ReportAsync(startDate, endDate)
        });
    }

    public async Task<IActionResult> ExportReport(DateTime? startDate, DateTime? endDate)
    {
        if (!HttpContext.Session.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "News");
        }

        var articles = await _newsService.ReportAsync(startDate, endDate);
        var csv = new StringBuilder();
        csv.AppendLine("NewsArticleID,Title,Category,CreatedBy,CreatedDate,Status");
        foreach (var article in articles)
        {
            csv.AppendLine(string.Join(",",
                EscapeCsv(article.NewsArticleID),
                EscapeCsv(article.NewsTitle),
                EscapeCsv(article.Category?.CategoryName),
                EscapeCsv(article.CreatedBy?.AccountName),
                EscapeCsv(article.CreatedDate.ToString("yyyy-MM-dd HH:mm")),
                EscapeCsv(article.NewsStatus ? "Active" : "Inactive")));
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"FUNews_Report_{DateTime.Now:yyyyMMddHHmm}.csv");
    }

    private static string EscapeCsv(string? value)
    {
        value ??= string.Empty;
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
