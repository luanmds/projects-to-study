using Domain.Model;
using Domain.Repositories;
using Infrastructure.Settings;

namespace Infrastructure.Repositories;

public class SecretRepository(SecretDbContext dbContext) : ISecretRepository
{
    public async Task SaveAsync(Secret secret)
    {
        await dbContext.Secrets.AddAsync(secret);
    }

    public Task GetById(string secretId)
    {
        throw new NotImplementedException();
    }
}