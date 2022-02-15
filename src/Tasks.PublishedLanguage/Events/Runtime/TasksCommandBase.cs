using System.Collections.Generic;
using MediatR;

namespace Tasks.PublishedLanguage.Events.Runtime
{
    public abstract record TasksCommandBase : IRequest
    {
        public Dictionary<string, string> AdditionalData { get; init; } = new Dictionary<string, string>();
    }
}