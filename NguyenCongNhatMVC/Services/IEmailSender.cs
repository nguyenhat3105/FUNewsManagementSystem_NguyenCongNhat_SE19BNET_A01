namespace NguyenCongNhatMVC.Services;

public interface IEmailSender
{
    Task<bool> TrySendAsync(string toEmail, string subject, string htmlBody);
}
