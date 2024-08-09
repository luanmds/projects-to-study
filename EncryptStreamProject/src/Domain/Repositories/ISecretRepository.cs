using Domain.Model;

namespace Domain.Repositories;

public interface ISecretRepository : IUnitOfWork
{
    Task AddAsync(Secret secret);

    Task<Secret> GetById(string secretId);

    void Update(Secret secret);
}