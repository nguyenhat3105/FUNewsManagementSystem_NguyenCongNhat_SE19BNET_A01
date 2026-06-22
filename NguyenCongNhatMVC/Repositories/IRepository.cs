using System.Linq.Expressions;

namespace NguyenCongNhatMVC.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(params object[] id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveAsync();
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}
