using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Repositories;

namespace NguyenCongNhatMVC.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _categories;
    private readonly IRepository<NewsArticle> _articles;

    public CategoryService(IRepository<Category> categories, IRepository<NewsArticle> articles)
    {
        _categories = categories;
        _articles = articles;
    }

    public Task<List<Category>> SearchAsync(string? keyword)
    {
        var query = _categories.Query().Include(c => c.ParentCategory).OrderBy(c => c.CategoryName).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => c.CategoryName.Contains(keyword) || c.CategoryDescription.Contains(keyword));
        }
        return query.ToListAsync();
    }

    public Task<List<Category>> ActiveAsync() => _categories.Query().Where(c => c.IsActive).OrderBy(c => c.CategoryName).ToListAsync();

    public Task<Category?> GetAsync(int id) => _categories.Query().FirstOrDefaultAsync(c => c.CategoryID == id);

    public async Task SaveAsync(Category category)
    {
        if (category.CategoryID == 0)
        {
            await _categories.AddAsync(category);
        }
        else
        {
            _categories.Update(category);
        }
        await _categories.SaveAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (await _articles.AnyAsync(n => n.CategoryID == id))
        {
            return false;
        }

        var category = await _categories.GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }

        _categories.Delete(category);
        await _categories.SaveAsync();
        return true;
    }
}
