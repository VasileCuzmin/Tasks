using MediatR;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents
{
    public record TaskFinished(TaskId TaskId, ProcessId ProcessId) : INotification;
}