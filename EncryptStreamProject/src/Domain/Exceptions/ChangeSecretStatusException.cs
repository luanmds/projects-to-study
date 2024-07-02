using System.Diagnostics.CodeAnalysis;
using Domain.Model.Enum;

namespace Domain.Exceptions;

[ExcludeFromCodeCoverage]
public class ChangeSecretStatusException(EncryptStatus actualStatus, EncryptStatus newStatus)
    : Exception($"Error in change secret status. Actual {actualStatus} | New {newStatus}");