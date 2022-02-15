using NBB.Messaging.Abstractions;
using System.Threading;
using Tasks.PublishedLanguage.Events;
using Tasks.PublishedLanguage.Events.Runtime;
using System.Threading.Tasks;


namespace Tasks.Worker
{
    public class MessageBusPublisherDecorator : IMessageBusPublisher
    {
        private readonly IMessageBusPublisher _innerPublisher;
        private readonly MessagingContextAccessor _messagingContextAccessor;

        public MessageBusPublisherDecorator(IMessageBusPublisher innerPublisher, MessagingContextAccessor messagingContextAccessor)
        {
            _innerPublisher = innerPublisher;
            _messagingContextAccessor = messagingContextAccessor;
        }

        public Task PublishAsync<T>(T message, MessagingPublisherOptions options = null, CancellationToken cancellationToken = default)
        {
            options ??= MessagingPublisherOptions.Default;

            if (_messagingContextAccessor.MessagingContext.MessagingEnvelope.Payload is TasksCommandBase command
                && message is TasksEventBase @event)
            {
                foreach (var (key, value) in command.AdditionalData)
                {
                    @event.AdditionalData[key] = value;
                }
            }

            return _innerPublisher.PublishAsync(message, options, cancellationToken);
        }
    }
}
