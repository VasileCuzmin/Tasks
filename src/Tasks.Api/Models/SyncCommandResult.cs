using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tasks.Api.Models
{
    public class SyncCommandResult
    {
        public List<EventDescriptor> Events { get; }
        public Guid CommandId { get; }

        private SyncCommandResult(List<EventDescriptor> events)
        {
            CommandId = Guid.NewGuid();
            Events = events;
        }

        public static SyncCommandResult From(IRequest command, IEnumerable<INotification> events)
            => new SyncCommandResult(events.Select(EventDescriptor.From).ToList());
    }

    public class EventDescriptor
    {
        public string EventType { get; }
        public INotification Payload { get; }

        private EventDescriptor(string eventType, INotification payload)
        {
            EventType = eventType;
            Payload = payload;
        }

        public static EventDescriptor From(INotification @event)
            => new EventDescriptor(@event.GetType().Name, @event);
    }
}