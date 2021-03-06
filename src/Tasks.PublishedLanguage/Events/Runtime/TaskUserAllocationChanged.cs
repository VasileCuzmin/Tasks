using System;
using System.Collections.Generic;

namespace Tasks.PublishedLanguage.Events.Runtime
{
    public record TaskUserAllocationChanged : TasksEventBase
    {
        public Guid TaskId { get; init; }
        public int? UserId { get; init; }
        public string ProcessId { get; init; }
    }
}