namespace Domain.Abstractions;

public abstract class AggregateRoot<T>
{
    public required string Id { get; init; }
}