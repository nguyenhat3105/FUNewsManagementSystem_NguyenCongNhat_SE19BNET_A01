using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public interface ICategoryService
{
    Task<List<Category>> SearchAsync(string? keyword);
    Task<List<Category>> ActiveAsync();
    Task<Category?> GetAsync(int id);
    Task SaveAsync(Category category);
    Task<bool> DeleteAsync(int id);
}
