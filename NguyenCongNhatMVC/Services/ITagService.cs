using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public interface ITagService
{
    Task<List<Tag>> AllAsync();
    Task<List<Tag>> SearchAsync(string? keyword);
    Task<Tag?> GetAsync(int id);
    Task SaveAsync(Tag tag);
    Task<bool> DeleteAsync(int id);
}
