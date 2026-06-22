using NguyenCongNhatMVC.Data;

namespace NguyenCongNhatMVC.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FUNewsDbContext _context;
    private readonly IServiceProvider _services;

    public UnitOfWork(FUNewsDbContext context, IServiceProvider services)
    {
        _context = context;
        _services = services;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        return _services.GetRequiredService<IRepository<T>>();
    }

    public Task SaveAsync() => _context.SaveChangesAsync();
}
