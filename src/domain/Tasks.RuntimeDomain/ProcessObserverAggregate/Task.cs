using System;
using System.Runtime.CompilerServices;
using NBB.Domain;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate.Snapshots;
using Tasks.Runtime.Domain.Services;

[assembly: InternalsVisibleTo("Tasks.RuntimeDomain.Tests")]

namespace Tasks.Runtime.Domain.ProcessObserverAggregate
{
    internal class Task : Entity<TaskId, ProcessObserverSnapshot.Task>
    {
        public TaskId TaskId { get; private set; }
        public TaskDefinition TaskDefinition { get; private set; }
        public TaskState State { get; private set; }

        // Needed for snapshoting
        internal Task()
        {
        }

        internal Task(TaskId taskId, TaskDefinition taskDefinition)
        {
            TaskId = taskId ?? throw new ArgumentNullException(nameof(taskId));
            TaskDefinition = taskDefinition ?? throw new ArgumentNullException(nameof(taskDefinition));
            State = TaskState.Initiated;
        }

        public (bool WillChange, TaskState NewState) EventChangesState(DynamicEvent @event, IExpressionEvaluationService expressionEvaluationService)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (expressionEvaluationService == null)
            {
                throw new ArgumentNullException(nameof(expressionEvaluationService));
            }

            if (@event.EventDefinitionName == TaskDefinition.CloseEvent && expressionEvaluationService.EvaluateEventExpression(@event, TaskDefinition.CloseExpression))
            {
                return (true, TaskState.Finished);
            }

            if (@event.EventDefinitionName == TaskDefinition.CancelEvent && expressionEvaluationService.EvaluateEventExpression(@event, TaskDefinition.CancelExpression))
            {
                return (true, TaskState.Canceled);
            }

            return (false, null);
        }

        public void ChangeState(TaskState newState)
        {
            State = newState;
        }

        public override TaskId GetIdentityValue() => TaskId;

        protected override void SetMemento(ProcessObserverSnapshot.Task memento)
        {
            TaskId = memento.TaskId;
            TaskDefinition = memento.TaskDefinition;
            State = memento.State;
        }

        protected override ProcessObserverSnapshot.Task CreateMemento()
        {
            return new ProcessObserverSnapshot.Task(TaskId, TaskDefinition, State);
        }
    }
}