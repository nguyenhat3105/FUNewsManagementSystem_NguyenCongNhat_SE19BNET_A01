using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public interface IArticleInteractionService
{
    Task<int> LikeCountAsync(string articleId);
    Task<int> BookmarkCountAsync(string articleId);
    Task<bool> UserLikedAsync(string articleId, int accountId);
    Task<bool> UserBookmarkedAsync(string articleId, int accountId);
    Task<List<NewsArticle>> BookmarkedArticlesAsync(int accountId);
    Task<List<ArticleComment>> CommentsAsync(string articleId);
    Task ToggleLikeAsync(string articleId, int accountId);
    Task ToggleBookmarkAsync(string articleId, int accountId);
    Task AddCommentAsync(string articleId, int? accountId, string displayName, string content, int? parentCommentId);
    Task EditCommentAsync(int commentId, int accountId, string content);
    Task DeleteCommentAsync(int commentId, int accountId);
}
