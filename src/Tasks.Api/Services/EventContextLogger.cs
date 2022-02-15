using MediatR;
using NBB.Core.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Api.Services
{
    public interface IEventHub
    {
        IEnumerable<INotification> GetEvents();
        void AddEvent(INotification @event);
    }

    class EventHub : IEventHub
    {
        private readonly HashSet<INotification> _events = new HashSet<INotification>();

        public EventHub()
        {
        }

        public void AddEvent(INotification @event)
        {
            _events.Add(@event);
        }

        public IEnumerable<INotification> GetEvents() => _events;
    }

    class EventContextLogger : INotificationHandler<INotification>
    {
        private readonly IEventHub _eventHub;

        public EventContextLogger(IEventHub eventHub)
        {
            _eventHub = eventHub;
        }

        public Task Handle(INotification notification, CancellationToken cancellationToken)
        {
            if (notification is INotification @event)
            {
                _eventHub.AddEvent(@event);
            }
            return Task.CompletedTask;
        }
    }
}
