using System.Diagnostics.CodeAnalysis;
using Application.Cqrs.Abstractions;

namespace Application.Cqrs;

[ExcludeFromCodeCoverage]
public class CreateSecret(string id, string correlationId, string secretTextRaw) : ICommand
{
    public string Id { get; set; } = id;

    public string CorrelationId { get; set; } = correlationId;

    public string SecretTextRaw  { get; set; } = secretTextRaw;
}