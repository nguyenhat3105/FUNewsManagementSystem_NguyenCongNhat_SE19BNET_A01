using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public interface ICurrentUserService
{
    AuthenticatedUser? User { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
