using Microsoft.AspNetCore.Mvc;
using NguyenCongNhatMVC.Services;

namespace NguyenCongNhatMVC.Controllers;

public class DashboardController : Controller
{
    private readonly IAnalyticsService _analyticsService;

    public DashboardController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    public async Task<IActionResult> Index()
    {
        var user = HttpContext.Session.GetUser();
        if (user?.Role is not ("Admin" or "Staff"))
        {
            return RedirectToAction("Index", "News");
        }

        try
        {
            return View(await _analyticsService.GetDashboardAsync());
        }
        catch (Exception)
        {
            TempData["Error"] = "Dashboard is unavailable because SQL Server cannot be reached.";
            return View(new Models.ViewModels.DashboardViewModel());
        }
    }
}
