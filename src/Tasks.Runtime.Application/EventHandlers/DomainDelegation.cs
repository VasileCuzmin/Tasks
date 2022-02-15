using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Runtime.Domain.ProcessObserverAggregate.DomainEvents;
using Tasks.Runtime.Domain.TaskAllocationAggregate;

namespace Tasks.Runtime.Application.EventHandlers
{
    public class DomainDelegation : INotificationHandler<TaskInitiated>,
                                    INotificationHandler<TaskCancelled>,
                                    INotificationHandler<TaskFinished>
    {
        private readonly IEventSourcedRepository<TaskAllocation> _taskAllocationRepository;

        public DomainDelegation(IEventSourcedRepository<TaskAllocation> taskAllocationRepository)
        {
            _taskAllocationRepository = taskAllocationRepository;
        }

        public async Task Handle(TaskInitiated notification, CancellationToken cancellationToken)
        {
            var taskAllocation = new TaskAllocation();
            taskAllocation.InitiateTask(notification.TaskId, notification.ProcessId, notification.TaskDefinition);
            await _taskAllocationRepository.SaveAsync(taskAllocation, cancellationToken);
        }

        public async Task Handle(TaskCancelled notification, CancellationToken cancellationToken)
        {
            var taskAllocation = await _taskAllocationRepository.GetByIdAsync(notification.TaskId, cancellationToken);
            taskAllocation.Cancel(notification.ProcessId);
            await _taskAllocationRepository.SaveAsync(taskAllocation, cancellationToken);
        }

        public async Task Handle(TaskFinished notification, CancellationToken cancellationToken)
        {
            var taskAllocation = await _taskAllocationRepository.GetByIdAsync(notification.TaskId, cancellationToken);
            taskAllocation.Finish(notification.ProcessId);
            await _taskAllocationRepository.SaveAsync(taskAllocation, cancellationToken);
        }
    }
}