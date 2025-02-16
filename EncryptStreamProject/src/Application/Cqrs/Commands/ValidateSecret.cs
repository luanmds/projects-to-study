﻿using System.Diagnostics.CodeAnalysis;
using Application.Cqrs.Abstractions;

namespace Application.Cqrs.Commands;

[ExcludeFromCodeCoverage]
public class ValidateSecret(string secretId, string correlationId) : ICommand
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; } = correlationId;
    public string SecretId { get; } = secretId;
}