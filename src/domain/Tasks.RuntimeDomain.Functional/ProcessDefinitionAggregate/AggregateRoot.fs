namespace Tasks.RuntimeDomain.Functional.ProcessDefinitionAggregate
open Tasks.RuntimeDomain.Functional.ProcessDefinitionAggregate.ValueObjects

type ProcessDefinition = {
    ProcessDefinitionId : ProcessDefinitionId
    TaskDefinitions: Set<TaskDefinition>
}



