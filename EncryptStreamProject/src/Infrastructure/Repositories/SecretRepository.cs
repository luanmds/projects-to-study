using Domain.Model;
using Domain.Repositories;
using Infrastructure.Settings;

namespace Infrastructure.Repositories;

public sealed class SecretRepository(SecretDbContext dbContext) : ISecretRepository, IDisposable
{
    private bool _disposed;
    
    public async Task AddAsync(Secret secret)
    {
        await dbContext.Secrets.AddAsync(secret);
    }

    public async Task<Secret?> GetById(string secretId)
    {
        return await dbContext.Secrets.FindAsync(secretId);
    }

    public void Update(Secret secret)
    {
        dbContext.Secrets.Update(secret);
    }

    public async Task Commit()
    {
        await dbContext.SaveChangesAsync();
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            dbContext.Dispose();
        }
        
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}