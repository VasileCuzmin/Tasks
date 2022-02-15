using MediatR;
using System.Collections.Generic;

namespace Tasks.Definition.Application.Events
{
    public record EventDefinitionUpdated(
        List<EventDefinitionUpdated.ProcessEventDefinitionModel> ProcessEventDefinitions,
        List<EventDefinitionUpdated.DeleteProcessEventDefinitionModel> ProcessEventDefinitionDeleted
    ) : INotification
    {
        public record ProcessEventDefinitionModel
        {
            public int ProcessDefinitionId { get; init; }
            public string ProcessIdentifierProps { get; init; }
            public EventDefinitionModel EventDefinition { get; init; }
        }

        public record DeleteProcessEventDefinitionModel
        {
            public int ProcessDefinitionId { get; init; }
            public int EventDefinitionId { get; init; }
        }

        public record EventDefinitionModel
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public string Topic { get; init; }
            public int ApplicationId { get; init; }
        }
    }
}
