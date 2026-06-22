using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using NguyenCongNhatMVC.Models.ViewModels;

namespace NguyenCongNhatMVC.Services;

public class EmailVerificationService : IEmailVerificationService
{
    private const string PendingKey = "PendingEmailRegistration";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailSender _emailSender;

    public EmailVerificationService(IHttpContextAccessor httpContextAccessor, IEmailSender emailSender)
    {
        _httpContextAccessor = httpContextAccessor;
        _emailSender = emailSender;
    }

    public async Task<EmailVerificationStartResult> StartAsync(RegisterViewModel model)
    {
        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        var expiresAt = DateTime.Now.AddMinutes(10);
        var pending = new PendingEmailRegistration(model, code, expiresAt);

        Session.SetString(PendingKey, JsonSerializer.Serialize(pending));

        var safeName = HtmlEncoder.Default.Encode(model.FullName.Trim());
        var body = $"""
            <p>Hello {safeName},</p>
            <p>Your FUNews verification code is:</p>
            <h2 style="letter-spacing:4px">{code}</h2>
            <p>This code expires in 10 minutes.</p>
            """;
        var sent = await _emailSender.TrySendAsync(model.Email.Trim(), "Verify your FUNews account", body);

        return new EmailVerificationStartResult(model.Email.Trim(), code, sent, expiresAt);
    }

    public bool TryComplete(string code, out RegisterViewModel? model, out string? error)
    {
        model = null;
        error = null;
        var json = Session.GetString(PendingKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            error = "Your verification session has expired. Please register again.";
            return false;
        }

        var pending = JsonSerializer.Deserialize<PendingEmailRegistration>(json);
        if (pending == null || pending.ExpiresAt < DateTime.Now)
        {
            Clear();
            error = "Verification code has expired. Please register again.";
            return false;
        }

        if (!string.Equals(pending.Code, code.Trim(), StringComparison.Ordinal))
        {
            error = "Verification code is incorrect.";
            return false;
        }

        model = pending.Model;
        return true;
    }

    public void Clear()
    {
        Session.Remove(PendingKey);
    }

    private ISession Session => _httpContextAccessor.HttpContext!.Session;

    private record PendingEmailRegistration(RegisterViewModel Model, string Code, DateTime ExpiresAt);
}
