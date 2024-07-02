using Application.Services;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

public static class ConfigureDependencies
{
    public static void ConfigureDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ISecretService, SecretService>();
        services.AddScoped<CryptorBuilder>();
    }
}