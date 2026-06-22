using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Repositories;

namespace NguyenCongNhatMVC.Services;

public class AuditService : IAuditService
{
    private readonly IRepository<AuditLog> _auditLogs;
    private readonly ICurrentUserService _currentUser;

    public AuditService(IRepository<AuditLog> auditLogs, ICurrentUserService currentUser)
    {
        _auditLogs = auditLogs;
        _currentUser = currentUser;
    }

    public async Task LogAsync(string action, string entityName, string? entityKey = null, string? description = null)
    {
        var user = _currentUser.User;
        try
        {
            await _auditLogs.AddAsync(new AuditLog
            {
                ActorEmail = user?.Email ?? "anonymous",
                ActorRole = user?.Role ?? "Guest",
                Action = action,
                EntityName = entityName,
                EntityKey = entityKey,
                Description = description,
                CreatedAt = DateTime.Now
            });
            await _auditLogs.SaveAsync();
        }
        catch
        {
            // Audit logging should never block the main user workflow.
        }
    }

    public Task<List<AuditLog>> LatestAsync(int take = 100)
    {
        return _auditLogs.Query()
            .OrderByDescending(a => a.CreatedAt)
            .Take(take)
            .ToListAsync();
    }
}
