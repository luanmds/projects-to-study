using System.Diagnostics.CodeAnalysis;
using Application.Services;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public static class ConfigureDependencies
{
    public static void ConfigureDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ISecretService, SecretService>();
        services.AddScoped<CryptorBuilder>();
    }
}