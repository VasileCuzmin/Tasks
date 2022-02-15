using MediatR;
using NBB.Messaging.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Runtime.Domain.TaskAllocationAggregate.DomainEvents;
using Tasks.RuntimeDomain.TaskAllocationAggregate.DomainEvents;

namespace Tasks.Runtime.Application.EventHandlers
{
    public class TasksDomainEventHandlers :
                    INotificationHandler<TaskStarted>,
                    INotificationHandler<TaskFinished>,
                    INotificationHandler<TaskCancelled>,
                    INotificationHandler<TaskAllocationCreated>,
                    INotificationHandler<TaskPaused>,
                    INotificationHandler<TaskUserAllocationChanged>,
                    INotificationHandler<TaskAllocatedAndStarted>
    {
        private readonly IMessageBusPublisher _messageBusPublisher;

        public TasksDomainEventHandlers(IMessageBusPublisher messageBusPublisher)
        {
            _messageBusPublisher = messageBusPublisher;
        }

        public Task Handle(TaskFinished notification, CancellationToken cancellationToken)
        {
            return _messageBusPublisher.PublishAsync(
                new PublishedLanguage.Events.Runtime.TaskFinished
                {
                    ProcessId = notification.ProcessId?.ToString(),
                    TaskId = notification.TaskId.Value
                }, cancellationToken);
        }

        public Task Handle(TaskCancelled notification, CancellationToken cancellationToken)
        {
            return _messageBusPublisher.PublishAsync(
                new PublishedLanguage.Events.Runtime.TaskCancelled
                {
                    ProcessId = notification.ProcessId?.ToString(),
                    TaskId = notification.TaskId.Value
                }, cancellationToken);
        }

        public Task Handle(TaskAllocationCreated notification, CancellationToken cancellationToken)
        {
            return _messageBusPublisher.PublishAsync(
                new PublishedLanguage.Events.Runtime.TaskAllocationCreated
                {
                    ProcessId = notification.ProcessId.ToString(),
                    TaskId = notification.TaskId.Value,
                    TaskDefinitionName = notification.TaskDefinition.Name,
                    GroupAllocationExpression = notification.TaskDefinition.GroupAllocationExpression?.Value,
                    UserAllocationExpression = notification.TaskDefinition.UserAllocationExpression?.Value
                }, cancellationToken);
        }

        public Task Handle(TaskPaused notification, CancellationToken cancellationToken)
        {
            return _messageBusPublisher.PublishAsync(
                new PublishedLanguage.Events.Runtime.TaskPaused
                {
                    ProcessId = notification.ProcessId.ToString(),
                    TaskId = notification.TaskId.Value
                }, cancellationToken);
        }

        public Task Handle(TaskStarted notification, CancellationToken cancellationToken)
        {
            return _messageBusPublisher.PublishAsync(
               new PublishedLanguage.Events.Runtime.TaskStarted
               {
                   ProcessId = notification.ProcessId.ToString(),
                   TaskId = notification.TaskId.Value
               }, cancellationToken);
        }

        public Task Handle(TaskUserAllocationChanged notification, CancellationToken cancellationToken)
        {
            return _messageBusPublisher.PublishAsync(
                new PublishedLanguage.Events.Runtime.TaskUserAllocationChanged
                {
                    ProcessId = notification.ProcessId.ToString(),
                    TaskId = notification.TaskId.Value,
                    UserId = notification.UserId
                }, cancellationToken);
        }

        public Task Handle(TaskAllocatedAndStarted notification, CancellationToken cancellationToken)
        {
            return _messageBusPublisher.PublishAsync(
                new PublishedLanguage.Events.Runtime.TaskAllocatedAndStarted
                {
                    ProcessId = notification.ProcessId.ToString(),
                    TaskId = notification.TaskId.Value,
                    UserId = notification.UserId
                }, cancellationToken);
        }
    }
}