namespace NguyenCongNhatMVC.Models.ViewModels;

public class NewsIndexViewModel
{
    public string? Keyword { get; set; }
    public int? CategoryID { get; set; }
    public int? TagID { get; set; }
    public bool IsAdminView { get; set; }
    public NewsArticle? FeaturedArticle { get; set; }
    public List<NewsArticle> Articles { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();
}
