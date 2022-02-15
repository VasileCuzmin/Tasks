using System;
using System.Collections.Generic;
using MediatR;

namespace Tasks.PublishedLanguage.Events
{
    public abstract record TasksEventBase : INotification
    {
        public Dictionary<string, string> AdditionalData { get; init; } = new Dictionary<string, string>();
    }

    public record EventMetadata
    {
        public Guid EventId { get; init; }
        public DateTime CreationDate { get; init; }

        public static EventMetadata Default() => new() { EventId = Guid.NewGuid(), CreationDate = DateTime.UtcNow };
    }
}
