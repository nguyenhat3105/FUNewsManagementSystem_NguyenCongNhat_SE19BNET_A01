namespace NguyenCongNhatMVC.Models;

public class NewsTag
{
    public string NewsArticleID { get; set; } = string.Empty;
    public int TagID { get; set; }

    public NewsArticle? NewsArticle { get; set; }
    public Tag? Tag { get; set; }
}
