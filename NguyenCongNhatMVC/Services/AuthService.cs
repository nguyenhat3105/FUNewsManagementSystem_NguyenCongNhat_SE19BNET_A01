using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NguyenCongNhatMVC.Data;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Models.ViewModels;
using NguyenCongNhatMVC.Repositories;

namespace NguyenCongNhatMVC.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<SystemAccount> _accounts;
    private readonly FUNewsDbContext _context;
    private readonly AdminAccountOptions _admin;

    public AuthService(
        IRepository<SystemAccount> accounts,
        FUNewsDbContext context,
        IOptions<AdminAccountOptions> admin)
    {
        _accounts = accounts;
        _context  = context;
        _admin    = admin.Value;
    }

    public async Task<AuthenticatedUser?> LoginAsync(string email, string password)
    {
        email    = email.Trim();
        password = password.Trim();

        if (email.Equals(_admin.Email, StringComparison.OrdinalIgnoreCase) && password == _admin.Password)
        {
            return new AuthenticatedUser { AccountID = 0, Email = _admin.Email, Name = _admin.Name, Role = "Admin" };
        }

        if (email.Equals(_admin.Email, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var account = await _accounts.Query()
            .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);

        if (account == null) return null;

        return new AuthenticatedUser
        {
            AccountID      = account.AccountID,
            Email          = account.AccountEmail,
            Name           = account.AccountName,
            Role           = account.AccountRole == 1 ? "Staff" : account.AccountRole == 2 ? "Lecturer" : "Reader",
            AvatarUrl      = account.AvatarUrl,
            StaffRoleValue = account.AccountRole
        };
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        email = email.Trim().ToLower();

        if (email.Equals(_admin.Email, StringComparison.OrdinalIgnoreCase)) return true;

        return await _context.SystemAccounts
            .AnyAsync(a => a.AccountEmail.ToLower() == email);
    }

    public async Task<AuthenticatedUser?> RegisterAsync(RegisterViewModel model)
    {
        var email = model.Email.Trim();

        // Prevent duplicate email
        if (await EmailExistsAsync(email)) return null;

        var account = new SystemAccount
        {
            AccountName     = model.FullName.Trim(),
            AccountEmail    = email,
            AccountPassword = model.Password.Trim(),
            AccountRole     = 3, // Reader
            PhoneNumber     = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim(),
            Bio             = string.IsNullOrWhiteSpace(model.Bio) ? null : model.Bio.Trim()
        };

        _context.SystemAccounts.Add(account);
        await _context.SaveChangesAsync();

        return new AuthenticatedUser
        {
            AccountID      = account.AccountID,
            Email          = account.AccountEmail,
            Name           = account.AccountName,
            Role           = "Reader",
            AvatarUrl      = account.AvatarUrl,
            StaffRoleValue = 3
        };
    }
}
