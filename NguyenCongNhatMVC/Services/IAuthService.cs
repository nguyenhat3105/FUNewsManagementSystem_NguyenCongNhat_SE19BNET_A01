using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Models.ViewModels;

namespace NguyenCongNhatMVC.Services;

public interface IAuthService
{
    Task<AuthenticatedUser?> LoginAsync(string email, string password);

    /// <summary>Registers a new Reader account. Returns null if email already exists.</summary>
    Task<AuthenticatedUser?> RegisterAsync(RegisterViewModel model);

    Task<bool> EmailExistsAsync(string email);
}
