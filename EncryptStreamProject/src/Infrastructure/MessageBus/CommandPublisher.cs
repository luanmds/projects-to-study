using System.Diagnostics.CodeAnalysis;
using Application.Cqrs.Abstractions;
using Application.Publishers;
using MediatR;

namespace Infrastructure.MessageBus;

[ExcludeFromCodeCoverage]
public class CommandPublisher(ISender sender) : ICommandPublisher
{
    public async Task Publish(ICommand command, CancellationToken cancellationToken = default)
    {
        await sender.Send(command, cancellationToken);
    }
}