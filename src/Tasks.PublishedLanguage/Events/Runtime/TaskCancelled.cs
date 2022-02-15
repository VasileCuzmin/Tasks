using System;
using System.Collections.Generic;

namespace Tasks.PublishedLanguage.Events.Runtime
{
    public record TaskCancelled : TasksEventBase
    {
        public Guid TaskId { get; init; }
        public string ProcessId { get; init; }
    }
}