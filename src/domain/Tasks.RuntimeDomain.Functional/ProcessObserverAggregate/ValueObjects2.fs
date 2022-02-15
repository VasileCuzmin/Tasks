namespace Tasks.RuntimeDomain.Functional.ProcessObserverAggregate.ValueObjects
open Tasks.RuntimeDomain.Functional.ProcessDefinitionAggregate.ValueObjects

open System

type Event = {
    EventDefinitionName: EventDefinitionName
    Json: string
    MessageId: Guid
    ApplicationName: string
}

type ImmutableKeyValue = {
    Key: string
    Value: string
}

type ProcessId = { KeyValues: ImmutableKeyValue list }

type ProcessObserverId = {
    ProcessDefinitionId: ProcessDefinitionId
    ProcessId: ProcessId
}

type TaskId = Guid


type TaskState = 
    | Started
    | Closed
    | Canceled

