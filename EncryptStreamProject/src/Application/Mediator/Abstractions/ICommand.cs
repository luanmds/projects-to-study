namespace Application.Mediator.Abstractions;

public interface ICommand
{
    public string Id { get; }
    public string CorrelationId { get; }
}