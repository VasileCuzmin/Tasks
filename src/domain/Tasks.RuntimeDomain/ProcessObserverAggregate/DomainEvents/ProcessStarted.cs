using System.Collections.Generic;
using MediatR;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents
{
    public record ProcessStarted(ProcessDefinitionId ProcessDefinitionId, ProcessId ProcessId, HashSet<TaskDefinition> TaskDefinitions) : INotification;
}