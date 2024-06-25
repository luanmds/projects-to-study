using Domain.Model;

namespace Domain.Repositories;

public interface ISecretRepository
{
    Task SaveAsync(Secret secret);

    Task<Secret?> GetById(string secretId);

    Task Update(Secret secret);
}