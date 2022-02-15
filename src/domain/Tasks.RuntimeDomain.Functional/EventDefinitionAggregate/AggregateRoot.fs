namespace Tasks.RuntimeDomain.Functional.EventDefinitionAggregate
open Tasks.RuntimeDomain.Functional.EventDefinitionAggregate.ValueObjects
open Tasks.RuntimeDomain.Functional.ProcessDefinitionAggregate.ValueObjects

type EventDefinition = { 
    Name : EventDefinitionName
    ProcessDefinitionsDictionary: Map<ProcessDefinitionId, ProcessIdentifierProps> 
}

