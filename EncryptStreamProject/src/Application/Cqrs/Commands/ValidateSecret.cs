using System.Diagnostics.CodeAnalysis;
using Application.Cqrs.Abstractions;

namespace Application.Cqrs.Commands;

[ExcludeFromCodeCoverage]
public class ValidateSecret(string id, string correlationId, string secretId) : ICommand
{
    public string Id { get; } = id;
    public string CorrelationId { get; } = correlationId;
    public string SecretId { get; } = secretId;
}