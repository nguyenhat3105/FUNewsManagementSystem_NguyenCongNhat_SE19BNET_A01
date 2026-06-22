using Microsoft.AspNetCore.Mvc;
using NguyenCongNhatMVC.Services;

namespace NguyenCongNhatMVC.Controllers;

public class AuditLogsController : Controller
{
    private readonly IAuditService _auditService;

    public AuditLogsController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task<IActionResult> Index()
    {
        if (!HttpContext.Session.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "News");
        }

        return View(await _auditService.LatestAsync(150));
    }
}
