using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Data;
using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public class ArticleWorkflowService : IArticleWorkflowService
{
    private readonly FUNewsDbContext _context;

    public ArticleWorkflowService(FUNewsDbContext context)
    {
        _context = context;
    }

    public Task<List<ApprovalHistory>> HistoryAsync(string articleId)
    {
        return _context.ApprovalHistories
            .Include(h => h.Account)
            .Where(h => h.NewsArticleID == articleId)
            .OrderByDescending(h => h.Timestamp)
            .ToListAsync();
    }

    public Task<bool> SubmitForReviewAsync(string articleId, int staffId)
    {
        return MoveAsync(articleId, staffId, 2, "Submit", "Submitted for review.");
    }

    public Task<bool> ApproveAsync(string articleId, int approverId, string? note)
    {
        return MoveAsync(articleId, approverId, 3, "Approve", note);
    }

    public Task<bool> RejectAsync(string articleId, int approverId, string? note)
    {
        return MoveAsync(articleId, approverId, 1, "Reject", note);
    }

    public Task<bool> ArchiveAsync(string articleId, int accountId, string? note)
    {
        return MoveAsync(articleId, accountId, 4, "Archive", note);
    }

    private async Task<bool> MoveAsync(string articleId, int accountId, int stateId, string action, string? note)
    {
        var article = await _context.NewsArticles.FirstOrDefaultAsync(n => n.NewsArticleID == articleId);
        if (article == null)
        {
            return false;
        }

        article.ArticleStateID = stateId;
        article.NewsStatus = stateId == 3;
        article.UpdatedByID = accountId == 0 ? null : accountId;
        article.ModifiedDate = DateTime.Now;
        _context.ApprovalHistories.Add(new ApprovalHistory
        {
            NewsArticleID = articleId,
            AccountID = accountId == 0 ? null : accountId,
            Action = action,
            Note = note,
            Timestamp = DateTime.Now
        });
        await _context.SaveChangesAsync();
        return true;
    }
}
