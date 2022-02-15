using MediatR;
using System.Collections.Generic;

namespace Tasks.PublishedLanguage.Events
{
    public record TasksCommandExecutionError : TasksEventBase
    {
        public string Code { get; init; }
        public object Data { get; init; }
    }
}
