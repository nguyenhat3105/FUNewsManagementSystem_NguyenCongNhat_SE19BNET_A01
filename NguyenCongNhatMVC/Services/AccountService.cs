using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Models;
using NguyenCongNhatMVC.Repositories;

namespace NguyenCongNhatMVC.Services;

public class AccountService : IAccountService
{
    private readonly IRepository<SystemAccount> _accounts;
    private readonly IRepository<NewsArticle> _articles;

    public AccountService(IRepository<SystemAccount> accounts, IRepository<NewsArticle> articles)
    {
        _accounts = accounts;
        _articles = articles;
    }

    public Task<List<SystemAccount>> SearchAsync(string? keyword)
    {
        var query = _accounts.Query().OrderBy(a => a.AccountName).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(a => a.AccountName.Contains(keyword) || a.AccountEmail.Contains(keyword));
        }
        return query.ToListAsync();
    }

    public Task<SystemAccount?> GetAsync(int id) => _accounts.Query().FirstOrDefaultAsync(a => a.AccountID == id);

    public Task<bool> EmailExistsAsync(string email)
    {
        email = email.Trim();
        return _accounts.AnyAsync(a => a.AccountEmail == email);
    }

    public async Task SaveAsync(SystemAccount account)
    {
        if (account.AccountID == 0)
        {
            await _accounts.AddAsync(account);
        }
        else
        {
            _accounts.Update(account);
        }
        await _accounts.SaveAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (await _articles.AnyAsync(n => n.CreatedByID == id || n.UpdatedByID == id))
        {
            return false;
        }

        var account = await _accounts.GetByIdAsync(id);
        if (account == null)
        {
            return false;
        }

        _accounts.Delete(account);
        await _accounts.SaveAsync();
        return true;
    }
}
