using MediatR;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents
{
    public record TaskPaused(ProcessId ProcessId, TaskId TaskId) : INotification;
}