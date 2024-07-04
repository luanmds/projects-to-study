using Domain.Events.Abstractions;
using MediatR;

namespace Application.EventHandlers.Abstractions;

public interface IEventHandler<in T> : IRequestHandler<T>  where T : IEvent, IRequest;