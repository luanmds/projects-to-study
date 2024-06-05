using MediatR;

namespace Application.Events.Abstractions;

public interface IEventHandler<T> : IRequestHandler<T>  where T : IEvent;