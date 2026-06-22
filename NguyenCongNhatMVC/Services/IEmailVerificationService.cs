using NguyenCongNhatMVC.Models.ViewModels;

namespace NguyenCongNhatMVC.Services;

public interface IEmailVerificationService
{
    Task<EmailVerificationStartResult> StartAsync(RegisterViewModel model);
    bool TryComplete(string code, out RegisterViewModel? model, out string? error);
    void Clear();
}

public record EmailVerificationStartResult(string Email, string Code, bool EmailSent, DateTime ExpiresAt);
