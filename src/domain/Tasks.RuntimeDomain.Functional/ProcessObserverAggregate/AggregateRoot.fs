namespace Tasks.RuntimeDomain.Functional.ProcessObserverAggregate

open Tasks.RuntimeDomain.Functional.ProcessObserverAggregate.ValueObjects
open Tasks.RuntimeDomain.Functional.ProcessObserverAggregate.DomainEvents
open Tasks.RuntimeDomain.Functional.ProcessObserverAggregate.Entities
open Tasks.RuntimeDomain.Functional.ProcessDefinitionAggregate.ValueObjects
open System

type ProcessObserver = {
    ProcessObserverId: ProcessObserverId
    TaskDefinitions: Set<TaskDefinition>
    ReceivedEvents: Set<Event>
    Tasks: Map<TaskId, Task>
    UnCommittedEvents: DomainEvent list
}

module ProcessObserver =  
    let apply domainEvent observer = 
        match domainEvent with 
            | ProcessStarted processStarted ->
                {observer with ProcessObserverId = observer.ProcessObserverId; TaskDefinitions = processStarted.TaskDefinitions; }
            | ProcessEventReceived processEventReceived ->
                {observer with ReceivedEvents = observer.ReceivedEvents |> Set.add processEventReceived.Event}
            | TaskStarted taskStarted ->
                let task = Task.create taskStarted.TaskId taskStarted.TaskDefinition
                {observer with Tasks = observer.Tasks |> Map.add task.TaskId task}
            | TaskStateChanged taskStateChanged ->
                let oldTask = observer.Tasks |> Map.find(taskStateChanged.TaskId)
                let newTask = oldTask |> Task.changeState taskStateChanged.NewState
                {observer with Tasks = (observer.Tasks |> Map.remove oldTask.TaskId |> Map.add newTask.TaskId newTask)}
            | TaskUserAllocationChanged taskUserAllocationChanged ->
                let oldTask = observer.Tasks |> Map.find(taskUserAllocationChanged.TaskId)
                let newTask = oldTask |> Task.allocateUser taskUserAllocationChanged.NewUserId
                {observer with Tasks = (observer.Tasks |> Map.remove oldTask.TaskId |> Map.add newTask.TaskId newTask)}
                
                
    let emit domainEvent observer = 
        {(observer |> apply domainEvent) 
            with UnCommittedEvents = observer.UnCommittedEvents |> List.append [domainEvent]} 

    let create processObserverId taskDefinitions =
        let observer = {
            ProcessObserverId = processObserverId
            TaskDefinitions = Set.empty
            ReceivedEvents = Set.empty
            Tasks = Map.empty
            UnCommittedEvents = List.empty
        }
        let de = ProcessStarted{
            ProcessDefinitionId = processObserverId.ProcessDefinitionId
            ProcessId = processObserverId.ProcessId
            TaskDefinitions = taskDefinitions
        }
        observer |> emit de

    let observeEvent event expressionEvaluatorFn observer =
        if  observer.ReceivedEvents.Contains(event) then observer
        else

            let receiveEvent obs =
                let de = ProcessEventReceived { 
                        ProcessId = obs.ProcessObserverId.ProcessId
                        Event = event 
                    }
                obs |> emit de

            let createTasks obs = 
                obs.TaskDefinitions 
                    |> Set.filter (fun taskDef -> taskDef.StartEvent = event.EventDefinitionName && expressionEvaluatorFn event taskDef.StartExpression)
                    |> Set.toList
                    |> List.map (fun taskDef -> TaskStarted {
                            ProcessId = obs.ProcessObserverId.ProcessId
                            TaskId = Guid.NewGuid()
                            TaskDefinition = taskDef
                            ReceivedEvent = event
                            ProcessDefinitionId = obs.ProcessObserverId.ProcessDefinitionId
                        })
                    |> List.fold (fun obs ev -> obs |> emit ev) obs

            let changeTasksState obs =
                 obs.Tasks
                |> Map.toSeq
                |> Seq.map (fun (_,t) -> t)
                |> Seq.filter (fun t -> t |> Task.changesStateWith event expressionEvaluatorFn)
                |> Seq.map (fun t -> TaskStateChanged {
                        ProcessId = obs.ProcessObserverId.ProcessId
                        TaskId = Guid.NewGuid()
                        OldState = t.TaskState
                        NewState = Task.getNewStateFor event expressionEvaluatorFn t
                        ReceivedEvent = event
                        ProcessDefinitionId = obs.ProcessObserverId.ProcessDefinitionId
                    })
                |> Seq.fold (fun obs ev -> obs |> emit ev) obs

            observer
                |> receiveEvent
                |> createTasks
                |> changeTasksState


            
            

    let allocateTaskToUser taskId userId observer =
        let task = observer.Tasks |> Map.find taskId;

        let de = TaskUserAllocationChanged {
            ProcessId = observer.ProcessObserverId.ProcessId
            TaskId = taskId
            OldUserId = task.UserId
            NewUserId= userId
        }
        observer |> emit de

