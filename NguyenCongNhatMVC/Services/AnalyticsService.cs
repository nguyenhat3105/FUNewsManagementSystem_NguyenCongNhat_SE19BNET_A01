using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Data;
using NguyenCongNhatMVC.Models.ViewModels;

namespace NguyenCongNhatMVC.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly FUNewsDbContext _context;

    public AnalyticsService(FUNewsDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        return new DashboardViewModel
        {
            TotalArticles = await _context.NewsArticles.CountAsync(),
            ActiveArticles = await _context.NewsArticles.CountAsync(n => n.NewsStatus),
            InactiveArticles = await _context.NewsArticles.CountAsync(n => !n.NewsStatus),
            TotalAccounts = await _context.SystemAccounts.CountAsync(),
            StaffAccounts = await _context.SystemAccounts.CountAsync(a => a.AccountRole == 1),
            LecturerAccounts = await _context.SystemAccounts.CountAsync(a => a.AccountRole == 2),
            TotalCategories = await _context.Categories.CountAsync(),
            TotalTags = await _context.Tags.CountAsync(),
            LatestArticles = await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .OrderByDescending(n => n.CreatedDate)
                .Take(6)
                .ToListAsync(),
            CategoryStats = await _context.NewsArticles
                .Include(n => n.Category)
                .GroupBy(n => n.Category!.CategoryName)
                .Select(g => new CategoryStatViewModel { CategoryName = g.Key, ArticleCount = g.Count() })
                .OrderByDescending(s => s.ArticleCount)
                .ToListAsync()
        };
    }
}
