using System;
using NBB.Domain;
using Tasks.Runtime.Domain.Constants;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents;
using Tasks.RuntimeDomain.TaskAllocationAggregate.DomainEvents;

namespace Tasks.Runtime.Domain.TaskAllocationAggregate
{
    public class TaskAllocation : EventSourcedAggregateRoot<TaskId>
    {
        public TaskId TaskId { get; private set; }
        public int? UserId { get; private set; }
        public TaskAllocationStatus State { get; private set; }
        public TaskDefinition TaskDefinition { get; private set; }

        //needed for es repository
        public TaskAllocation()
        {
        }
        public void InitiateTask(TaskId taskId, ProcessId processId, TaskDefinition taskDefinition)
        {
            if (taskId == null)
            {
                throw new ArgumentNullException(nameof(taskId));
            }

            Emit(new TaskAllocationCreated(taskId, taskDefinition, processId));
        }

        public void Cancel(ProcessId processId)
        {
            Emit(new TaskCancelled(TaskId, processId));
        }

        public void Finish(ProcessId processId)
        {
            Emit(new TaskFinished(TaskId, processId));
        }

        public void AllocateUser(string processId, int? userId)
        {
            if (State == TaskAllocationStatus.Finished || State == TaskAllocationStatus.Cancelled)
            {
                throw new DomainException(ErrorCodes.TaskFinnishedOrCancelled.ToString());
            }

            if (TaskDefinition.AutomaticStart.Value.GetValueOrDefault())
            {
                Emit(new TaskAllocatedAndStarted(processId, TaskId, userId));
            }
            else
            {
                Emit(new TaskUserAllocationChanged(processId, TaskId, userId));
            }
        }

        public void Start(string processId)
        {
            if (State == TaskAllocationStatus.InProgress)
            {
                throw new DomainException(ErrorCodes.TaskAlreadyStarted.ToString());
            }

            if (State == TaskAllocationStatus.Finished || State == TaskAllocationStatus.Cancelled)
            {
                throw new DomainException(ErrorCodes.TaskFinnishedOrCancelled.ToString());
            }

            if (!UserId.HasValue)
            {
                throw new DomainException(ErrorCodes.TaskHasNoAllocatedUser.ToString());
            }

            Emit(new TaskStarted(new ProcessId(processId), TaskId));
        }

        public void Pause(string processId)
        {
            if (State == TaskAllocationStatus.InStandby)
            {
                throw new DomainException(ErrorCodes.TaskAlreadyInStandby.ToString());
            }

            if (State == TaskAllocationStatus.Finished || State == TaskAllocationStatus.Cancelled)
            {
                throw new DomainException(ErrorCodes.TaskFinnishedOrCancelled.ToString());
            }

            if (!UserId.HasValue)
            {
                throw new DomainException(ErrorCodes.TaskHasNoAllocatedUser.ToString());
            }

            Emit(new TaskPaused(new ProcessId(processId), TaskId));
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void Apply(TaskAllocationCreated e)
#pragma warning restore IDE0051 // Remove unused private members
        {
            TaskDefinition = e.TaskDefinition;
            TaskId = e.TaskId;
            State = TaskAllocationStatus.Created;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void Apply(TaskAllocatedAndStarted e)
#pragma warning restore IDE0051 // Remove unused private members
        {
            TaskId = e.TaskId;
            UserId = e.UserId;
            State = TaskAllocationStatus.InProgress;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void Apply(TaskFinished e)
#pragma warning restore IDE0051 // Remove unused private members
        {
            State = TaskAllocationStatus.Finished;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void Apply(TaskCancelled e)
#pragma warning restore IDE0051 // Remove unused private members
        {
            State = TaskAllocationStatus.Cancelled;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void Apply(TaskUserAllocationChanged e)
#pragma warning restore IDE0051 // Remove unused private members
        {
            UserId = e.UserId;

            if (State != TaskAllocationStatus.Allocated)
            {
                State = TaskAllocationStatus.Allocated;
            }
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void Apply(TaskStarted e)
#pragma warning restore IDE0051 // Remove unused private members
        {
            State = TaskAllocationStatus.InProgress;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void Apply(TaskPaused e)
#pragma warning restore IDE0051 // Remove unused private members
        {
            State = TaskAllocationStatus.InStandby;
        }

        public override TaskId GetIdentityValue() => TaskId;
    }
}