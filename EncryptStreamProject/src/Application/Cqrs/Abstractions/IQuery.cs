using Domain.Model.Abstractions;
using MediatR;

namespace Application.Cqrs.Abstractions;

public interface IQuery<T> : IRequest<T> where T : AggregateRoot<T>
{

}