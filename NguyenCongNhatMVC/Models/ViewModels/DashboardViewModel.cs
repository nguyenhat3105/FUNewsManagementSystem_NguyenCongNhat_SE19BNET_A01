namespace NguyenCongNhatMVC.Models.ViewModels;

public class DashboardViewModel
{
    public int TotalArticles { get; set; }
    public int ActiveArticles { get; set; }
    public int InactiveArticles { get; set; }
    public int TotalAccounts { get; set; }
    public int StaffAccounts { get; set; }
    public int LecturerAccounts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalTags { get; set; }
    public List<NewsArticle> LatestArticles { get; set; } = new();
    public List<CategoryStatViewModel> CategoryStats { get; set; } = new();
}
