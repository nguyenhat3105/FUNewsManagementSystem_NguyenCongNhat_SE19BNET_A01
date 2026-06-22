using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public AuthenticatedUser? User => _httpContextAccessor.HttpContext?.Session.GetUser();

    public bool IsAuthenticated => User != null;

    public bool IsInRole(string role) => User?.Role == role;
}
