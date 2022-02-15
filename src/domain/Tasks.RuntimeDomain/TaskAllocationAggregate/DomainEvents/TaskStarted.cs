using MediatR;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents
{
    public record TaskStarted(ProcessId ProcessId, TaskId TaskId) : INotification;
}