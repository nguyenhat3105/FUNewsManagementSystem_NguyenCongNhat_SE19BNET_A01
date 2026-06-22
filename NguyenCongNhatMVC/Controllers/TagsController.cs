using Microsoft.AspNetCore.Mvc;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Services;

namespace NguyenCongNhatMVC.Controllers;

public class TagsController : Controller
{
    private readonly ITagService _tagService;
    private readonly IAuditService _auditService;

    public TagsController(ITagService tagService, IAuditService auditService)
    {
        _tagService = tagService;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index(string? keyword)
    {
        if (!HttpContext.Session.IsInRole("Staff"))
        {
            return RedirectToAction("Index", "News");
        }

        ViewBag.Keyword = keyword;
        return View(await _tagService.SearchAsync(keyword));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Tag tag)
    {
        if (!HttpContext.Session.IsInRole("Staff"))
        {
            return Forbid();
        }

        // Navigation properties are not posted — remove from validation
        ModelState.Remove("NewsTags");
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please check all tag fields.";
            return RedirectToAction(nameof(Index));
        }

        var action = tag.TagID == 0 ? "Create" : "Update";
        await _tagService.SaveAsync(tag);
        await _auditService.LogAsync(action, "Tag", tag.TagID.ToString(), tag.TagName);
        TempData["Success"] = "Tag saved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!HttpContext.Session.IsInRole("Staff"))
        {
            return Forbid();
        }

        if (await _tagService.DeleteAsync(id))
        {
            await _auditService.LogAsync("Delete", "Tag", id.ToString());
            TempData["Success"] = "Tag deleted.";
        }
        else
        {
            TempData["Error"] = "Cannot delete a tag that is used by news articles.";
        }

        return RedirectToAction(nameof(Index));
    }
}
