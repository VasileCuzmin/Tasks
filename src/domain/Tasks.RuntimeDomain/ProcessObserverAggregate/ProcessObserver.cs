using System;
using System.Collections.Generic;
using System.Linq;
using NBB.Core.Abstractions;
using NBB.Domain;
using Newtonsoft.Json;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents;
using Tasks.Runtime.Domain.ProcessObserverAggregate.Snapshots;
using Tasks.Runtime.Domain.Services;
using ProcessEventReceived = Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents.ProcessEventReceived;
using ProcessStarted = Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents.ProcessStarted;
using TaskCancelled = Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents.TaskCancelled;
using TaskInitiated = Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents.TaskInitiated;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate
{
    public class ProcessObserver : EventSourcedAggregateRoot<ProcessObserverId, ProcessObserverSnapshot>
    {
        public ProcessObserverId ProcessObserverId { get; private set; }

        private HashSet<TaskDefinition> _taskDefinitions;
        private readonly HashSet<JsonEvent> _receivedEvents = new HashSet<JsonEvent>();


        private readonly Dictionary<TaskId, Task> _tasks = new Dictionary<TaskId, Task>();

        //needed for es repository
        public ProcessObserver()
        {
        }

        public static ProcessObserver New(ProcessObserverId processObserverId, HashSet<TaskDefinition> taskDefinitions)
        {
            if (processObserverId == null)
            {
                throw new ArgumentNullException(nameof(processObserverId));
            }

            if (taskDefinitions == null)
            {
                throw new ArgumentNullException(nameof(taskDefinitions));
            }

            var ps = new ProcessObserver();
            ps.Emit(new ProcessStarted(processObserverId.ProcessDefinitionId, processObserverId.ProcessId, taskDefinitions));
            return ps;
        }

        public void ObserveEvent(DynamicEvent @event, IExpressionEvaluationService expressionEvaluationService)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (expressionEvaluationService == null)
            {
                throw new ArgumentNullException(nameof(expressionEvaluationService));
            }

            var jsonEvent = new JsonEvent(@event.EventDefinitionName, JsonConvert.SerializeObject(@event.Payload), @event.MessageId, @event.ApplicationName);

            if (_receivedEvents.Contains(jsonEvent))
            {
                return;
            }

            // TODO: need to test and implement logic for out of order events!

            Emit(new ProcessEventReceived(ProcessObserverId.ProcessId, jsonEvent));

            var initiatedTasks = _taskDefinitions
                .Where(t => t.StartEvent == @event.EventDefinitionName && expressionEvaluationService.EvaluateEventExpression(@event, t.StartExpression))
                .Select(t => new TaskInitiated(ProcessObserverId.ProcessId, new TaskId(), t));

            foreach (var initiatedTask in initiatedTasks)
            {
                Emit(initiatedTask);
            }

            var finishedTasks = _tasks.Values
                .Where(t => t.State != TaskState.Finished)
                .Select(t => new { EventChangesState = t.EventChangesState(@event, expressionEvaluationService), Task = t })
                .Where(t => t.EventChangesState.WillChange)
                .Where(t => t.EventChangesState.NewState == TaskState.Finished)
                .Select(t => new TaskFinished(t.Task.TaskId, ProcessObserverId.ProcessId))
                .ToList();

            foreach (var finishedTask in finishedTasks)
            {
                if (_tasks.ContainsKey(finishedTask.TaskId))
                {
                    Emit(finishedTask);
                }
            }

            var cancelledTasks = _tasks.Values
                .Where(t => t.State != TaskState.Canceled)
                .Select(t => new
                { EventChangesState = t.EventChangesState(@event, expressionEvaluationService), Task = t })
                .Where(t => t.EventChangesState.WillChange)
                .Where(t => t.EventChangesState.NewState == TaskState.Canceled)
                .Select(t => new TaskCancelled(t.Task.TaskId, ProcessObserverId.ProcessId))
                .ToList();

            foreach (var cancelledTask in cancelledTasks)
            {
                if (_tasks.ContainsKey(cancelledTask.TaskId))
                {
                    Emit(cancelledTask);
                }
            }
        }

        #region Apply
        private void Apply(ProcessStarted e)
        {
            ProcessObserverId = new ProcessObserverId(e.ProcessDefinitionId, e.ProcessId);
            _taskDefinitions = e.TaskDefinitions;
        }

        private void Apply(ProcessEventReceived e)
        {
            _receivedEvents.Add(e.Event);
        }

        private void Apply(TaskInitiated e)
        {
            _tasks[e.TaskId] = new Task(e.TaskId, e.TaskDefinition);
        }

        private void Apply(TaskCancelled e)
        {
            _tasks[e.TaskId].ChangeState(TaskState.Canceled);
        }

        private void Apply(TaskFinished e)
        {
            _tasks[e.TaskId].ChangeState(TaskState.Finished);
        }
        #endregion

        public override ProcessObserverId GetIdentityValue() => ProcessObserverId;

        protected override void SetMemento(ProcessObserverSnapshot memento)
        {
            ProcessObserverId = memento.ProcessObserverId;

            _taskDefinitions = new HashSet<TaskDefinition>();
            _taskDefinitions.UnionWith(memento.TaskDefinitions);
            _receivedEvents.UnionWith(memento.ReceivedEvents);

            foreach (var mementoTask in memento.Tasks)
            {
                var task = new Task();
                ((IMementoProvider<ProcessObserverSnapshot.Task>)task).SetMemento(mementoTask);

                _tasks.Add(task.TaskId, task);
            }
        }

        protected override ProcessObserverSnapshot CreateMemento()
        {
            var memento = new ProcessObserverSnapshot(
                ProcessObserverId,
                _taskDefinitions.ToList(),
                _receivedEvents.ToList(),
                _tasks.Values
                    .Select(x => ((IMementoProvider<ProcessObserverSnapshot.Task>)x).CreateMemento())
                    .ToList()
            );

            return memento;
        }
    }
}