using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public interface IAccountService
{
    Task<List<SystemAccount>> SearchAsync(string? keyword);
    Task<SystemAccount?> GetAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task SaveAsync(SystemAccount account);
    Task<bool> DeleteAsync(int id);
}
