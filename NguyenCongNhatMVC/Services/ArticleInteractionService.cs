using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Data;
using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public class ArticleInteractionService : IArticleInteractionService
{
    private readonly FUNewsDbContext _context;
    private readonly INotificationService _notificationService;

    public ArticleInteractionService(FUNewsDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public Task<int> LikeCountAsync(string articleId) =>
        _context.ArticleLikes.CountAsync(l => l.NewsArticleID == articleId);

    public Task<int> BookmarkCountAsync(string articleId) =>
        _context.ArticleBookmarks.CountAsync(b => b.NewsArticleID == articleId);

    public Task<List<NewsArticle>> BookmarkedArticlesAsync(int accountId)
    {
        return _context.NewsArticles
            .Where(n => n.Bookmarks.Any(b => b.AccountID == accountId))
            .Include(n => n.Category)
            .Include(n => n.ArticleState)
            .Include(n => n.CreatedBy)
            .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
            .OrderByDescending(n => n.Bookmarks.Where(b => b.AccountID == accountId).Max(b => b.CreatedAt))
            .ToListAsync();
    }

    public async Task<List<ArticleComment>> CommentsAsync(string articleId)
    {
        var comments = await _context.ArticleComments
            .AsNoTracking()
            .Include(c => c.Account)
            .Where(c => c.NewsArticleID == articleId && !c.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        var lookup = comments.ToDictionary(c => c.ArticleCommentID);
        foreach (var comment in comments) comment.Replies = new List<ArticleComment>();
        foreach (var comment in comments)
        {
            if (comment.ParentCommentID.HasValue && lookup.TryGetValue(comment.ParentCommentID.Value, out var parent))
                parent.Replies.Add(comment);
        }

        return comments
            .Where(c => c.ParentCommentID == null || !lookup.ContainsKey(c.ParentCommentID.Value))
            .OrderByDescending(c => c.CreatedAt)
            .ToList();
    }

    public async Task ToggleLikeAsync(string articleId, int accountId)
    {
        var like = await _context.ArticleLikes
            .FirstOrDefaultAsync(l => l.NewsArticleID == articleId && l.AccountID == accountId);

        if (like == null)
        {
            _context.ArticleLikes.Add(new ArticleLike { NewsArticleID = articleId, AccountID = accountId });
            await _context.SaveChangesAsync();

            // Push notification to article author
            var article = await _context.NewsArticles
                .Include(n => n.CreatedBy)
                .FirstOrDefaultAsync(n => n.NewsArticleID == articleId);
            if (article != null && article.CreatedByID != accountId)
            {
                var actor = await _context.SystemAccounts.FindAsync(accountId);
                await _notificationService.PushAsync(
                    articleId, article.CreatedByID, "Like",
                    accountId, actor?.AccountName ?? "Someone");
            }
        }
        else
        {
            _context.ArticleLikes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ToggleBookmarkAsync(string articleId, int accountId)
    {
        var bookmark = await _context.ArticleBookmarks
            .FirstOrDefaultAsync(b => b.NewsArticleID == articleId && b.AccountID == accountId);

        if (bookmark == null)
        {
            _context.ArticleBookmarks.Add(new ArticleBookmark { NewsArticleID = articleId, AccountID = accountId });
            await _context.SaveChangesAsync();

            // Push notification to article author
            var article = await _context.NewsArticles
                .Include(n => n.CreatedBy)
                .FirstOrDefaultAsync(n => n.NewsArticleID == articleId);
            if (article != null && article.CreatedByID != accountId)
            {
                var actor = await _context.SystemAccounts.FindAsync(accountId);
                await _notificationService.PushAsync(
                    articleId, article.CreatedByID, "Bookmark",
                    accountId, actor?.AccountName ?? "Someone");
            }
        }
        else
        {
            _context.ArticleBookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddCommentAsync(string articleId, int? accountId, string displayName, string content, int? parentCommentId)
    {
        _context.ArticleComments.Add(new ArticleComment
        {
            NewsArticleID  = articleId,
            AccountID      = accountId == 0 ? null : accountId,
            DisplayName    = displayName.Trim(),
            Content        = content.Trim(),
            ParentCommentID = parentCommentId,
            CreatedAt      = DateTime.Now
        });
        await _context.SaveChangesAsync();

        // Push notification to article author (and to parent comment author if reply)
        var article = await _context.NewsArticles.FirstOrDefaultAsync(n => n.NewsArticleID == articleId);
        if (article != null && accountId.HasValue && accountId.Value != 0)
        {
            var excerpt = content.Length > 80 ? content[..80] + "…" : content;
            await _notificationService.PushAsync(
                articleId, article.CreatedByID, "Comment",
                accountId, displayName, excerpt);

            if (parentCommentId.HasValue)
            {
                var parent = await _context.ArticleComments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.ArticleCommentID == parentCommentId.Value);

                if (parent?.AccountID is int parentAccountId && parentAccountId != article.CreatedByID)
                {
                    await _notificationService.PushAsync(
                        articleId, parentAccountId, "Reply",
                        accountId, displayName, excerpt);
                }
            }
        }
    }

    public Task<bool> UserLikedAsync(string articleId, int accountId) =>
        _context.ArticleLikes.AnyAsync(l => l.NewsArticleID == articleId && l.AccountID == accountId);

    public Task<bool> UserBookmarkedAsync(string articleId, int accountId) =>
        _context.ArticleBookmarks.AnyAsync(b => b.NewsArticleID == articleId && b.AccountID == accountId);

    public async Task EditCommentAsync(int commentId, int accountId, string content)
    {
        var comment = await _context.ArticleComments
            .FirstOrDefaultAsync(c => c.ArticleCommentID == commentId && c.AccountID == accountId && !c.IsDeleted);
        if (comment == null) return;
        comment.Content = content.Trim();
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(int commentId, int accountId)
    {
        var comment = await _context.ArticleComments
            .FirstOrDefaultAsync(c => c.ArticleCommentID == commentId && c.AccountID == accountId && !c.IsDeleted);
        if (comment == null) return;
        comment.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}
