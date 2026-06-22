namespace NguyenCongNhatMVC.Repositories;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : class;
    Task SaveAsync();
}
