using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Data;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Models.ViewModels;
using NguyenCongNhatMVC.Repositories;

namespace NguyenCongNhatMVC.Services;

public class NewsService : INewsService
{
    private readonly FUNewsDbContext _context;
    private readonly IRepository<NewsArticle> _articles;

    public NewsService(FUNewsDbContext context, IRepository<NewsArticle> articles)
    {
        _context = context;
        _articles = articles;
    }

    public Task<List<NewsArticle>> SearchAsync(string? keyword, bool activeOnly = false, int? createdById = null, int? categoryId = null, int? tagId = null)
    {
        var query = IncludeDetails(_articles.Query());
        if (activeOnly)
        {
            var now = DateTime.Now;
            query = query.Where(n => n.ArticleStateID == 3 && n.NewsStatus && (!n.ScheduledPublishDate.HasValue || n.ScheduledPublishDate <= now));
        }
        if (createdById.HasValue)
        {
            query = query.Where(n => n.CreatedByID == createdById.Value);
        }
        if (categoryId.HasValue)
        {
            query = query.Where(n => n.CategoryID == categoryId.Value);
        }
        if (tagId.HasValue)
        {
            query = query.Where(n => n.NewsTags.Any(nt => nt.TagID == tagId.Value));
        }
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(n => n.NewsTitle.Contains(keyword) || n.Headline.Contains(keyword) || n.NewsContent.Contains(keyword));
        }
        return query.OrderByDescending(n => n.CreatedDate).ToListAsync();
    }

    public Task<List<NewsArticle>> ReportAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = IncludeDetails(_articles.Query());
        if (startDate.HasValue)
        {
            query = query.Where(n => n.CreatedDate.Date >= startDate.Value.Date);
        }
        if (endDate.HasValue)
        {
            query = query.Where(n => n.CreatedDate.Date <= endDate.Value.Date);
        }
        return query.OrderByDescending(n => n.CreatedDate).ToListAsync();
    }

    public Task<List<ArticleState>> StatesAsync() => _context.ArticleStates.OrderBy(s => s.ArticleStateID).ToListAsync();

    public Task<NewsArticle?> GetAsync(string id) => IncludeDetails(_articles.Query()).FirstOrDefaultAsync(n => n.NewsArticleID == id);

    public async Task SaveAsync(ArticleViewModel model, int staffId)
    {
        NewsArticle article;
        if (string.IsNullOrWhiteSpace(model.NewsArticleID))
        {
            article = new NewsArticle
            {
                NewsArticleID = await NextIdAsync(),
                CreatedByID = staffId,
                CreatedDate = DateTime.Now
            };
            await _articles.AddAsync(article);
        }
        else
        {
            article = await _context.NewsArticles.Include(n => n.NewsTags).FirstAsync(n => n.NewsArticleID == model.NewsArticleID);
            article.UpdatedByID = staffId;
            article.ModifiedDate = DateTime.Now;
            article.NewsTags.Clear();
        }

        article.NewsTitle = model.NewsTitle;
        article.Headline = model.Headline;
        article.NewsContent = model.NewsContent;
        article.NewsSource = model.NewsSource;
        article.ImageUrl = model.ImageUrl;
        article.CategoryID = model.CategoryID;
        article.NewsStatus = model.NewsStatus;
        article.ArticleStateID = model.ArticleStateID;
        article.ScheduledPublishDate = model.ScheduledPublishDate;
        if (article.ArticleStateID != 3)
        {
            article.NewsStatus = false;
        }

        foreach (var tagId in model.TagIDs.Distinct())
        {
            article.NewsTags.Add(new NewsTag { NewsArticleID = article.NewsArticleID, TagID = tagId });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var article = await _context.NewsArticles.Include(n => n.NewsTags).FirstOrDefaultAsync(n => n.NewsArticleID == id);
        if (article == null)
        {
            return false;
        }

        _context.NewsTags.RemoveRange(article.NewsTags);
        _context.NewsArticles.Remove(article);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(string id, int staffId)
    {
        var article = await _context.NewsArticles.FirstOrDefaultAsync(n => n.NewsArticleID == id);
        if (article == null)
        {
            return false;
        }

        article.NewsStatus = !article.NewsStatus;
        article.UpdatedByID = staffId;
        article.ModifiedDate = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<NewsArticle> IncludeDetails(IQueryable<NewsArticle> query)
    {
        return query
            .Include(n => n.Category)
            .Include(n => n.ArticleState)
            .Include(n => n.CreatedBy)
            .Include(n => n.NewsTags)
            .ThenInclude(nt => nt.Tag);
    }

    private async Task<string> NextIdAsync()
    {
        var count = await _context.NewsArticles.CountAsync() + 1;
        string id;
        do
        {
            id = $"N{count:000000000}";
            count++;
        }
        while (await _context.NewsArticles.AnyAsync(n => n.NewsArticleID == id));

        return id;
    }
}
