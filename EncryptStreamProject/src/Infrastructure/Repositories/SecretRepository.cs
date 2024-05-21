using Domain.Model;
using Domain.Repositories;

namespace Infrastructure.Repositories;

public class SecretRepository : ISecretRepository
{
    public Task SaveAsync(Secret secret)
    {
        throw new NotImplementedException();
    }

    public Task GetById(string secretId)
    {
        throw new NotImplementedException();
    }
}