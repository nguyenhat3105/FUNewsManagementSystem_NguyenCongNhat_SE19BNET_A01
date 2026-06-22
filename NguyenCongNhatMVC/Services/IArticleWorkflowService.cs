using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public interface IArticleWorkflowService
{
    Task<List<ApprovalHistory>> HistoryAsync(string articleId);
    Task<bool> SubmitForReviewAsync(string articleId, int staffId);
    Task<bool> ApproveAsync(string articleId, int approverId, string? note);
    Task<bool> RejectAsync(string articleId, int approverId, string? note);
    Task<bool> ArchiveAsync(string articleId, int accountId, string? note);
}
