namespace Domain.Repositories;

public interface IUnitOfWork
{
    Task Commit();
}