using MediatR;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents
{
    public record ProcessEventReceived(ProcessId ProcessId, JsonEvent Event) : INotification;
}