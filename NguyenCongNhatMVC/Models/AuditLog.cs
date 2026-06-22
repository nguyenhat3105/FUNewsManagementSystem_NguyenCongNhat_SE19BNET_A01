using System.ComponentModel.DataAnnotations;

namespace NguyenCongNhatMVC.Models;

public class AuditLog
{
    public int AuditLogID { get; set; }

    [Required, StringLength(80)]
    public string ActorEmail { get; set; } = string.Empty;

    [Required, StringLength(30)]
    public string ActorRole { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string Action { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string EntityName { get; set; } = string.Empty;

    [StringLength(80)]
    public string? EntityKey { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
