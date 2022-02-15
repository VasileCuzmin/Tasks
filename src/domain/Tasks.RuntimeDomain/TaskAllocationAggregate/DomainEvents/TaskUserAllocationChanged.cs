using MediatR;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents
{
    // TODO: why not ProcessId type
    public record TaskUserAllocationChanged(string ProcessId, TaskId TaskId,  int? UserId) : INotification;
}