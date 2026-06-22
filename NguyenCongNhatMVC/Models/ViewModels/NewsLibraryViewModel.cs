namespace NguyenCongNhatMVC.Models.ViewModels;

public class NewsLibraryViewModel
{
    public bool IsStaff { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<NewsArticle> CreatedArticles { get; set; } = new();
    public List<NewsArticle> BookmarkedArticles { get; set; } = new();
}
