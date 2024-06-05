using MediatR;

namespace Application.Cqrs.Abstractions;

public interface ICommand : IRequest
{
    public string Id { get; }
    public string CorrelationId { get; }
}