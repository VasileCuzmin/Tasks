using System;
using System.Collections.Generic;

namespace Tasks.PublishedLanguage.Events.Runtime
{
    public record TaskStarted : TasksEventBase
    {
        public Guid TaskId { get; init; }
        public string ProcessId { get; init; }
    }
}