using System;
using Tasks.PublishedLanguage.Events.Runtime;

namespace Tasks.Runtime.Application.Commands
{
    public record PauseTask : TasksCommandBase
    {
        public Guid TaskId { get; init; }
        public string ProcessId { get; init; }
    }
}