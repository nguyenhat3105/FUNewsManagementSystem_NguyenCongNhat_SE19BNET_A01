using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Data;
using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public class NotificationService : INotificationService
{
    private readonly FUNewsDbContext _context;

    public NotificationService(FUNewsDbContext context)
    {
        _context = context;
    }

    public Task<List<Notification>> GetForUserAsync(int accountId, int take = 30)
    {
        return _context.Notifications
            .Where(n => n.RecipientAccountID == accountId)
            .Include(n => n.NewsArticle)
            .Include(n => n.Actor)
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .ToListAsync();
    }

    public Task<Notification?> GetByIdForUserAsync(int notificationId, int accountId)
    {
        return _context.Notifications
            .Include(n => n.NewsArticle)
            .FirstOrDefaultAsync(n => n.NotificationID == notificationId && n.RecipientAccountID == accountId);
    }

    public Task<int> UnreadCountAsync(int accountId)
    {
        return _context.Notifications
            .CountAsync(n => n.RecipientAccountID == accountId && !n.IsRead);
    }

    public async Task MarkAllReadAsync(int accountId)
    {
        var unread = await _context.Notifications
            .Where(n => n.RecipientAccountID == accountId && !n.IsRead)
            .ToListAsync();
        foreach (var n in unread) n.IsRead = true;
        await _context.SaveChangesAsync();
    }

    public async Task MarkReadAsync(int notificationId)
    {
        var n = await _context.Notifications.FindAsync(notificationId);
        if (n != null) { n.IsRead = true; await _context.SaveChangesAsync(); }
    }

    public async Task PushAsync(string articleId, int recipientId, string type, int? actorId, string actorName, string? excerpt = null)
    {
        // Không gửi thông báo cho chính mình
        if (actorId.HasValue && actorId.Value == recipientId) return;

        // Deduplicate: không push cùng type từ cùng actor trong 1 phút
        var recent = await _context.Notifications.AnyAsync(n =>
            n.NewsArticleID == articleId &&
            n.RecipientAccountID == recipientId &&
            n.Type == type &&
            n.ActorAccountID == actorId &&
            n.CreatedAt > DateTime.Now.AddMinutes(-1));

        if (recent) return;

        _context.Notifications.Add(new Notification
        {
            RecipientAccountID = recipientId,
            NewsArticleID      = articleId,
            Type               = type,
            ActorAccountID     = actorId,
            ActorName          = actorName,
            Excerpt            = excerpt?.Length > 120 ? excerpt[..120] + "…" : excerpt,
            IsRead             = false,
            CreatedAt          = DateTime.Now
        });

        await _context.SaveChangesAsync();
    }
}
