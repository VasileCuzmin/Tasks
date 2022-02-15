using System.Collections.Generic;
using NBB.Domain;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;

namespace Tasks.Runtime.Domain.EventDefinitionAggregate
{
    public class EventDefinition : EventedAggregateRoot<EventDefinitionName>
    {
        public EventDefinitionName EventDefinitionName { get; private set; }
        public Dictionary<ProcessDefinitionId, IdentifierPropsMap> ProcessDefinitionsDictionary { get; private set; }

        public EventDefinition(EventDefinitionName eventDefinitionName, Dictionary<ProcessDefinitionId, IdentifierPropsMap> processDefinitionsDictionary)
        {
            EventDefinitionName = eventDefinitionName;
            ProcessDefinitionsDictionary = processDefinitionsDictionary;
        }

        public override EventDefinitionName GetIdentityValue() => EventDefinitionName;
    }
}