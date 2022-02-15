using System;

namespace Tasks.PublishedLanguage.Events.Runtime
{
    public record TaskAllocationCreated : TasksEventBase
    {
        public string ProcessId { get; init; }
        public Guid TaskId { get; init; }
        public string TaskDefinitionName { get; init; }
        public string GroupAllocationExpression { get; init; }
        public string UserAllocationExpression { get; init; }

        public EventMetadata Metadata { get; init; } = EventMetadata.Default();
    }
}