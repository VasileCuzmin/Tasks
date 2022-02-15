namespace Tasks.RuntimeDomain.Functional.ProcessDefinitionAggregate.ValueObjects

type ProcessDefinitionId = int

type DynamicExpression = string

type EventDefinitionName = string

type TaskDefinition = { 
    Name : string
    StartEvent: EventDefinitionName
    StartExpression: DynamicExpression
    CloseEvent: EventDefinitionName
    CloseExpression: DynamicExpression
    CancelEvent: EventDefinitionName
    CancelExpression: DynamicExpression
    GroupAllocationExpression: DynamicExpression
    UserAllocationExpression: DynamicExpression
}