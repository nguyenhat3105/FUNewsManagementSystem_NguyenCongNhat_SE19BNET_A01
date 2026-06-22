using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Models.ViewModels;

namespace NguyenCongNhatMVC.Services;

public interface INewsService
{
    Task<List<NewsArticle>> SearchAsync(string? keyword, bool activeOnly = false, int? createdById = null, int? categoryId = null, int? tagId = null);
    Task<List<NewsArticle>> ReportAsync(DateTime? startDate, DateTime? endDate);
    Task<List<ArticleState>> StatesAsync();
    Task<NewsArticle?> GetAsync(string id);
    Task SaveAsync(ArticleViewModel model, int staffId);
    Task<bool> ToggleStatusAsync(string id, int staffId);
    Task<bool> DeleteAsync(string id);
}
