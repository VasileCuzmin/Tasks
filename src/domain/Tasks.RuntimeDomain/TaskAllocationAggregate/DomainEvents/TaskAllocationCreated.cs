using MediatR;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents
{
    public record TaskAllocationCreated(TaskId TaskId, TaskDefinition TaskDefinition, ProcessId ProcessId) : INotification;
    
}