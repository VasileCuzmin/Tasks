using System;
using System.Collections.Generic;

namespace Tasks.PublishedLanguage.Events.Runtime
{
    public record TaskPaused : TasksEventBase
    {
        public string ProcessId { get; init; }
        public Guid TaskId { get; init; }

        public EventMetadata Metadata { get; init; } = EventMetadata.Default();
    }
}