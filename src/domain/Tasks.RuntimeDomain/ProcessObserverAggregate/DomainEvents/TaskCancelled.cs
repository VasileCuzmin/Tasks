using MediatR;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents
{
    public record TaskCancelled(TaskId TaskId, ProcessId ProcessId) : INotification;
}