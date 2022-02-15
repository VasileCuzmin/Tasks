using System;

namespace Tasks.PublishedLanguage.Events.Runtime
{
    public record TaskAllocatedAndStarted : TasksEventBase
    {
        public Guid TaskId { get; init; }
        public int? UserId { get; init; }
        public string ProcessId { get; init; }
    }
}