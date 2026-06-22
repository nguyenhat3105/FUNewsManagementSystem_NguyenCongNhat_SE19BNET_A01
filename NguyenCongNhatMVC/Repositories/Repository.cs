using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NguyenCongNhatMVC.Data;

namespace NguyenCongNhatMVC.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly FUNewsDbContext _context;

    public Repository(FUNewsDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> Query() => _context.Set<T>();

    public Task<List<T>> GetAllAsync() => _context.Set<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(params object[] id) => await _context.Set<T>().FindAsync(id);

    public Task AddAsync(T entity) => _context.Set<T>().AddAsync(entity).AsTask();

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public void Delete(T entity) => _context.Set<T>().Remove(entity);

    public Task SaveAsync() => _context.SaveChangesAsync();

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => _context.Set<T>().AnyAsync(predicate);
}
