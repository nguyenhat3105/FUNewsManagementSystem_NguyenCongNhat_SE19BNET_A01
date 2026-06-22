namespace NguyenCongNhatMVC.Models;

public class Notification
{
    public int NotificationID { get; set; }
    public int RecipientAccountID { get; set; }   // tác giả nhận thông báo
    public string NewsArticleID { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Like" | "Bookmark" | "Comment"
    public int? ActorAccountID { get; set; }          // người thực hiện hành động
    public string ActorName { get; set; } = string.Empty;
    public string? Excerpt { get; set; }              // đoạn nội dung comment (nếu có)
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public NewsArticle? NewsArticle { get; set; }
    public SystemAccount? Recipient { get; set; }
    public SystemAccount? Actor { get; set; }
}
