namespace NguyenCongNhatMVC.Models;

public class ArticleBookmark
{
    public int ArticleBookmarkID { get; set; }
    public string NewsArticleID { get; set; } = string.Empty;
    public int AccountID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public NewsArticle? NewsArticle { get; set; }
    public SystemAccount? Account { get; set; }
}
