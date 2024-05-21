using System.Linq.Expressions;
using Domain.Model.Abstractions;

namespace Application.Mediator;

public interface IQuery<T,TResult> where T : AggregateRoot<T>
{
    Task<TResult> Get(Expression<Func<T, bool>> filter);
}