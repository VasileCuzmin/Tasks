namespace Tasks.RuntimeDomain.Functional.ProcessObserverAggregate.Entities
open Tasks.RuntimeDomain.Functional.ProcessDefinitionAggregate.ValueObjects
open Tasks.RuntimeDomain.Functional.ProcessObserverAggregate.ValueObjects

[<NoEquality; NoComparison>]
type Task = {
    TaskId: TaskId
    TaskDefinition: TaskDefinition
    TaskState: TaskState
    UserId: option<int>
}

module Task = 
    let create taskId taskDefinition = 
        { 
            TaskId = taskId;
            TaskDefinition = taskDefinition
            TaskState = Started
            UserId = None
        }   

    let getNewStateFor event expressionEvaluatorFn task = 
        match task.TaskState with
            | Started when event.EventDefinitionName = task.TaskDefinition.CloseEvent && expressionEvaluatorFn event task.TaskDefinition.CloseExpression -> Closed
            | Started when event.EventDefinitionName = task.TaskDefinition.CancelEvent && expressionEvaluatorFn event task.TaskDefinition.CancelExpression -> Canceled
            | other -> other

    let changesStateWith event expressionEvaluatorFn task = 
        task.TaskState <> (getNewStateFor event expressionEvaluatorFn task)

    let changeState taskState task = 
        {task with TaskState = taskState}

    let allocateUser userId task = 
        {task with UserId = userId}

