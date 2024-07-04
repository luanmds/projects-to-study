namespace Domain.Model.Abstractions;

public abstract class AggregateRoot<T>(string id)
{
    public string Id { get; init; } = id;
}