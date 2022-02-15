using System.Collections.Generic;
using NBB.Domain;

namespace Tasks.Runtime.Domain.ProcessDefinitionAggregate
{
    public class ProcessDefinition : EventedAggregateRoot<ProcessDefinitionId>
    {
        public ProcessDefinitionId ProcessDefinitionId { get; }
        public HashSet<TaskDefinition> TaskDefinitions { get; }

        public ProcessDefinition(ProcessDefinitionId processDefinitionId, HashSet<TaskDefinition> taskDefinitions)
        {
            ProcessDefinitionId = processDefinitionId;
            TaskDefinitions = taskDefinitions;
        }

        public override ProcessDefinitionId GetIdentityValue() => ProcessDefinitionId;
    }
}