using MediatR;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents
{
    public record TaskFinished(TaskId TaskId, ProcessId ProcessId) : INotification;
}