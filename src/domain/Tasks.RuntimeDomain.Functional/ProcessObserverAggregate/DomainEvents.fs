module Tasks.RuntimeDomain.Functional.ProcessObserverAggregate.DomainEvents
open Tasks.RuntimeDomain.Functional.ProcessObserverAggregate.ValueObjects
open Tasks.RuntimeDomain.Functional.ProcessDefinitionAggregate.ValueObjects

type ProcessStartedDomainEvent = {
    ProcessDefinitionId: ProcessDefinitionId
    ProcessId: ProcessId
    TaskDefinitions: Set<TaskDefinition>
}

type ProcessEventReceivedDomainEvent = {
    ProcessId: ProcessId
    Event: Event
}

type TaskStartedDomainEvent = {
    ProcessId: ProcessId
    TaskId: TaskId
    TaskDefinition: TaskDefinition
    ReceivedEvent: Event
    ProcessDefinitionId: ProcessDefinitionId
}

type TaskStateChangedDomainEvent = {
    ProcessId: ProcessId
    TaskId: TaskId
    OldState: TaskState
    NewState: TaskState
    ReceivedEvent: Event
    ProcessDefinitionId: ProcessDefinitionId
}

type TaskUserAllocationChangedDomainEvent = {
    ProcessId: ProcessId
    TaskId: TaskId
    OldUserId: option<int>
    NewUserId: option<int>
}

type DomainEvent = 
    | ProcessStarted of ProcessStartedDomainEvent
    | ProcessEventReceived of ProcessEventReceivedDomainEvent
    | TaskStarted of TaskStartedDomainEvent
    | TaskStateChanged of TaskStateChangedDomainEvent
    | TaskUserAllocationChanged of TaskUserAllocationChangedDomainEvent

