using Microsoft.AspNetCore.Mvc;
using NguyenCongNhatMVC.Services;

namespace NguyenCongNhatMVC.Controllers;

public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index()
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0)
            return RedirectToAction("Login", "Auth");

        await _notificationService.MarkAllReadAsync(user.AccountID);
        var notifications = await _notificationService.GetForUserAsync(user.AccountID, 50);
        return View(notifications);
    }

    /// <summary>GET /Notifications/Feed — returns JSON for the bell dropdown</summary>
    public async Task<IActionResult> Feed()
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0)
            return Json(new { items = Array.Empty<object>(), unread = 0 });

        var items  = await _notificationService.GetForUserAsync(user.AccountID, 15);
        var unread = items.Count(n => !n.IsRead);

        var result = items.Select(n => new
        {
            id           = n.NotificationID,
            type         = n.Type,
            actor        = n.ActorName,
            excerpt      = n.Excerpt,
            articleId    = n.NewsArticleID,
            articleTitle = n.NewsArticle?.NewsTitle ?? "",
            isRead       = n.IsRead,
            createdAt    = TimeAgo(n.CreatedAt)
        });

        return Json(new { items = result, unread });
    }

    /// <summary>POST /Notifications/MarkAllRead — mark all as read, returns JSON</summary>
    [HttpPost]
    public async Task<IActionResult> MarkAllRead()
    {
        var user = HttpContext.Session.GetUser();
        if (user != null && user.AccountID > 0)
            await _notificationService.MarkAllReadAsync(user.AccountID);
        return Json(new { ok = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _notificationService.MarkReadAsync(id);
        return Ok();
    }

    public async Task<IActionResult> Open(int id)
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0)
            return RedirectToAction("Login", "Auth");

        var notification = await _notificationService.GetByIdForUserAsync(id, user.AccountID);
        if (notification == null)
            return RedirectToAction(nameof(Index));

        await _notificationService.MarkReadAsync(id);
        return RedirectToAction("Details", "News", new { id = notification.NewsArticleID });
    }

    private static string TimeAgo(DateTime dt)
    {
        var diff = DateTime.Now - dt;
        if (diff.TotalMinutes < 1)  return "just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalHours   < 24) return $"{(int)diff.TotalHours}h ago";
        if (diff.TotalDays    < 7)  return $"{(int)diff.TotalDays}d ago";
        return dt.ToString("dd MMM, HH:mm");
    }
}
