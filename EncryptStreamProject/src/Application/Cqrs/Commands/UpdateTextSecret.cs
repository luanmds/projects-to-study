using Application.Cqrs.Abstractions;

namespace Application.Cqrs.Commands;

public class UpdateTextSecret(string id, string correlationId, string secretId, string textEncrypted) 
    : ICommand
{
    public string Id { get; } = id;
    public string CorrelationId { get; } = correlationId;
    public string SecretId { get; } = secretId;
    public string TextEncrypted { get; } = textEncrypted;
}