using Domain.Model;

namespace Domain.Repositories;

public interface ISecretRepository
{
    Task SaveAsync(Secret secret);

    Task GetById(string secretId);
}