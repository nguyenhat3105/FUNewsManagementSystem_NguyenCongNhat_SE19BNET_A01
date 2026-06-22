using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Models.ViewModels;
using NguyenCongNhatMVC.Services;

namespace NguyenCongNhatMVC.Controllers;

public class NewsController : Controller
{
    private readonly INewsService _newsService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;
    private readonly IAuditService _auditService;
    private readonly IArticleWorkflowService _workflowService;
    private readonly IArticleInteractionService _interactionService;

    public NewsController(
        INewsService newsService,
        ICategoryService categoryService,
        ITagService tagService,
        IAuditService auditService,
        IArticleWorkflowService workflowService,
        IArticleInteractionService interactionService)
    {
        _newsService = newsService;
        _categoryService = categoryService;
        _tagService = tagService;
        _auditService = auditService;
        _workflowService = workflowService;
        _interactionService = interactionService;
    }

    public async Task<IActionResult> Index(string? keyword, int? categoryId, int? tagId)
    {
        try
        {
            var user = HttpContext.Session.GetUser();
            var isAdmin = user?.Role == "Admin";
            var articles = await _newsService.SearchAsync(keyword, activeOnly: !isAdmin, categoryId: categoryId, tagId: tagId);

            // Admin sees all except pure Draft (state 1) — those belong to authors
            if (isAdmin)
                articles = articles.Where(a => a.ArticleStateID != 1).ToList();

            return View(new NewsIndexViewModel
            {
                Keyword = keyword,
                CategoryID = categoryId,
                TagID = tagId,
                IsAdminView = isAdmin,
                FeaturedArticle = isAdmin ? null : articles.FirstOrDefault(),
                Articles = isAdmin ? articles : articles.Skip(1).ToList(),
                Categories = await _categoryService.ActiveAsync(),
                Tags = await _tagService.AllAsync()
            });
        }
        catch (Exception)
        {
            TempData["Error"] = "Login succeeded, but the application cannot connect to SQL Server. Please check the connection string or SQL Server authentication.";
            return View(new NewsIndexViewModel
            {
                Keyword = keyword,
                CategoryID = categoryId,
                TagID = tagId
            });
        }
    }

    public async Task<IActionResult> Manage(string? keyword)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Staff")
        {
            return RedirectToAction(nameof(Index));
        }

        await FillArticleListsAsync();
        ViewBag.Keyword = keyword;
        return View(await _newsService.SearchAsync(keyword));
    }

    public async Task<IActionResult> Details(string id)
    {
        var article = await _newsService.GetAsync(id);
        var user = HttpContext.Session.GetUser();
        var isPublishedNow = article?.ArticleStateID == 3 && article.NewsStatus && (!article.ScheduledPublishDate.HasValue || article.ScheduledPublishDate <= DateTime.Now);
        if (article == null || (!isPublishedNow && user?.Role is not ("Staff" or "Admin")))
        {
            return NotFound();
        }
        ViewBag.LikeCount     = await _interactionService.LikeCountAsync(id);
        ViewBag.BookmarkCount = await _interactionService.BookmarkCountAsync(id);
        ViewBag.Comments      = await _interactionService.CommentsAsync(id);
        ViewBag.ApprovalHistory = await _workflowService.HistoryAsync(id);
        if (user != null && user.AccountID > 0)
        {
            ViewBag.UserLiked      = await _interactionService.UserLikedAsync(id, user.AccountID);
            ViewBag.UserBookmarked = await _interactionService.UserBookmarkedAsync(id, user.AccountID);
        }
        return View(article);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(ArticleViewModel model)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Staff")
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please check all article fields.";
            return RedirectToAction(nameof(Manage));
        }

        var action = string.IsNullOrWhiteSpace(model.NewsArticleID) ? "Create" : "Update";
        await _newsService.SaveAsync(model, user.AccountID);
        await _auditService.LogAsync(action, "NewsArticle", model.NewsArticleID, model.NewsTitle);
        TempData["Success"] = "Article saved.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitForReview(string id)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Staff")
        {
            return Forbid();
        }

        if (await _workflowService.SubmitForReviewAsync(id, user.AccountID))
        {
            await _auditService.LogAsync("SubmitForReview", "NewsArticle", id);
            TempData["Success"] = "Article submitted for review.";
        }
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string id, string? note)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Admin")
        {
            return Forbid();
        }

        if (await _workflowService.ApproveAsync(id, user.AccountID, note))
        {
            await _auditService.LogAsync("Approve", "NewsArticle", id, note);
            TempData["Success"] = "Article approved and published.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(string id, string? note)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Admin")
        {
            return Forbid();
        }

        if (await _workflowService.RejectAsync(id, user.AccountID, note))
        {
            await _auditService.LogAsync("Reject", "NewsArticle", id, note);
            TempData["Success"] = "Article rejected and moved back to draft.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (!HttpContext.Session.IsInRole("Staff"))
        {
            return Forbid();
        }

        if (await _newsService.DeleteAsync(id))
        {
            await _auditService.LogAsync("Delete", "NewsArticle", id);
            TempData["Success"] = "Article deleted.";
        }
        return RedirectToAction(nameof(Manage));
    }

    public async Task<IActionResult> History()
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Staff")
        {
            return RedirectToAction(nameof(Index));
        }

        return View(await _newsService.SearchAsync(null, createdById: user.AccountID));
    }

    public async Task<IActionResult> Drafts()
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Staff")
        {
            return RedirectToAction(nameof(Index));
        }

        var articles = await _newsService.SearchAsync(null, createdById: user.AccountID);
        return View(articles.Where(n => !n.NewsStatus).ToList());
    }

    public async Task<IActionResult> Library()
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0)
        {
            return RedirectToAction("Login", "Auth");
        }

        var createdArticles = user.Role == "Staff"
            ? await _newsService.SearchAsync(null, createdById: user.AccountID)
            : new List<NewsArticle>();

        return View(new NewsLibraryViewModel
        {
            IsStaff = user.Role == "Staff",
            UserName = user.Name,
            CreatedArticles = createdArticles,
            BookmarkedArticles = await _interactionService.BookmarkedArticlesAsync(user.AccountID)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role != "Staff")
        {
            return Forbid();
        }

        if (await _newsService.ToggleStatusAsync(id, user.AccountID))
        {
            await _auditService.LogAsync("ToggleStatus", "NewsArticle", id);
            TempData["Success"] = "Article status updated.";
        }
        else
        {
            TempData["Error"] = "Article not found.";
        }

        return Redirect(Request.Headers.Referer.ToString() ?? Url.Action(nameof(Manage))!);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLike(string id)
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0)
        {
            TempData["Error"] = "Please login with a system account to like articles.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await _interactionService.ToggleLikeAsync(id, user.AccountID);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBookmark(string id)
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0)
        {
            TempData["Error"] = "Please login with a system account to bookmark articles.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await _interactionService.ToggleBookmarkAsync(id, user.AccountID);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(string id, string displayName, string content, int? parentCommentId)
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0)
        {
            TempData["Error"] = "Please login with a system account to comment.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(content))
        {
            TempData["Error"] = "Display name and comment content are required.";
            return RedirectToAction(nameof(Details), new { id });
        }

        await _interactionService.AddCommentAsync(id, user.AccountID, displayName, content, parentCommentId);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditComment(string id, int commentId, string content)
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0) return Forbid();
        if (!string.IsNullOrWhiteSpace(content))
            await _interactionService.EditCommentAsync(commentId, user.AccountID, content);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(string id, int commentId)
    {
        var user = HttpContext.Session.GetUser();
        if (user == null || user.AccountID <= 0) return Forbid();
        await _interactionService.DeleteCommentAsync(commentId, user.AccountID);
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task FillArticleListsAsync()
    {
        ViewBag.Categories = new SelectList(await _categoryService.ActiveAsync(), "CategoryID", "CategoryName");
        ViewBag.Tags = await _tagService.AllAsync();
        ViewBag.States = new SelectList(await _newsService.StatesAsync(), "ArticleStateID", "StateName");
    }
}
