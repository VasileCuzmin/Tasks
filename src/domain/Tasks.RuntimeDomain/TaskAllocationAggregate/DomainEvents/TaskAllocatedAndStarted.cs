using MediatR;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.RuntimeDomain.TaskAllocationAggregate.DomainEvents
{
    // TODO: why not ProcessId type
    public record TaskAllocatedAndStarted(string ProcessId, TaskId TaskId, int? UserId) : INotification;
}