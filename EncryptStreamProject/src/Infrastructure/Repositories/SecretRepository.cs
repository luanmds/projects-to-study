using Domain.Model;
using Domain.Repositories;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SecretRepository(SecretDbContext dbContext) : ISecretRepository
{
    public async Task SaveAsync(Secret secret)
    {
        await dbContext.Secrets.AddAsync(secret);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Secret?> GetById(string secretId)
    {
        return await dbContext.Secrets.FindAsync(secretId);
    }

    public async Task Update(Secret secret)
    {
        dbContext.Secrets.Update(secret);
        await dbContext.SaveChangesAsync();
    }
}