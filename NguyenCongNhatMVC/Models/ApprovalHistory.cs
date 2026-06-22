using System.ComponentModel.DataAnnotations;

namespace NguyenCongNhatMVC.Models;

public class ApprovalHistory
{
    public int HistoryID { get; set; }

    [Required, StringLength(20)]
    public string NewsArticleID { get; set; } = string.Empty;

    public int? AccountID { get; set; }

    [Required, StringLength(30)]
    public string Action { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Note { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public NewsArticle? NewsArticle { get; set; }
    public SystemAccount? Account { get; set; }
}
