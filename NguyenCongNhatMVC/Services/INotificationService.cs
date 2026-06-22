using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public interface INotificationService
{
    Task<List<Notification>> GetForUserAsync(int accountId, int take = 30);
    Task<Notification?> GetByIdForUserAsync(int notificationId, int accountId);
    Task<int> UnreadCountAsync(int accountId);
    Task MarkAllReadAsync(int accountId);
    Task MarkReadAsync(int notificationId);
    Task PushAsync(string articleId, int recipientId, string type, int? actorId, string actorName, string? excerpt = null);
}
