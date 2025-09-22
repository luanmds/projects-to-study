using System.Diagnostics.CodeAnalysis;
using Application.Cqrs.Commands;
using Application.Publishers;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Endpoints;

[ExcludeFromCodeCoverage]
public static class ApiEndpointsExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapPost("/send", async ([FromServices] ICommandPublisher publisher) =>
        {
            await publisher.Publish(
                new CreateSecret(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                "secret example"));
        });

        app.MapGet("/secrets/all", async ([FromServices] SecretDbContext db) =>
        {
            var secrets = await db.Secrets.ToListAsync();
            return Results.Ok(secrets);
        });

        app.MapDelete("/secrets/delete", async ([FromServices] SecretDbContext db) =>
        {
            db.Secrets.RemoveRange(db.Secrets);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
    }
}
