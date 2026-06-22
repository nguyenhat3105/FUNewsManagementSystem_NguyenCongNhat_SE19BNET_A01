using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Services;

namespace NguyenCongNhatMVC.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IAuditService _auditService;

    public CategoriesController(ICategoryService categoryService, IAuditService auditService)
    {
        _categoryService = categoryService;
        _auditService = auditService;
    }

    public async Task<IActionResult> Index(string? keyword)
    {
        if (!HttpContext.Session.IsInRole("Staff"))
        {
            return RedirectToAction("Index", "News");
        }

        ViewBag.Keyword = keyword;
        ViewBag.ParentCategories = new SelectList(await _categoryService.SearchAsync(null), "CategoryID", "CategoryName");
        return View(await _categoryService.SearchAsync(keyword));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Category category)
    {
        if (!HttpContext.Session.IsInRole("Staff"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please check all category fields.";
            return RedirectToAction(nameof(Index));
        }

        var action = category.CategoryID == 0 ? "Create" : "Update";
        await _categoryService.SaveAsync(category);
        await _auditService.LogAsync(action, "Category", category.CategoryID.ToString(), category.CategoryName);
        TempData["Success"] = "Category saved.";
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

        if (await _categoryService.DeleteAsync(id))
        {
            await _auditService.LogAsync("Delete", "Category", id.ToString());
            TempData["Success"] = "Category deleted.";
        }
        else
        {
            TempData["Error"] = "Cannot delete a category that is used by news articles.";
        }
        return RedirectToAction(nameof(Index));
    }
}
