using Application.Services;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

public static class ConfigureDependencies
{
    public static void ConfigureDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<ISecretRepository, SecretRepository>();
        services.AddScoped<ISecretService, SecretService>();
    }
}