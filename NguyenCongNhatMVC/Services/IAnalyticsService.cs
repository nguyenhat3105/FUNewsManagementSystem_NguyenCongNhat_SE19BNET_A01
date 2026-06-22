using NguyenCongNhatMVC.Models.ViewModels;

namespace NguyenCongNhatMVC.Services;

public interface IAnalyticsService
{
    Task<DashboardViewModel> GetDashboardAsync();
}
