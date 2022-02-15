using MediatR;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents
{
    public record TaskInitiated(ProcessId ProcessId, TaskId TaskId, TaskDefinition TaskDefinition) : INotification;
}