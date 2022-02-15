using MediatR;
using System.Collections.Generic;

namespace Tasks.Definition.Application.Events
{
    public record TaskUpdated(List<TaskUpdated.TaskDefinitionModel> TaskDefinition) : INotification
    {
        public record EventDefinitionModel
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public string Topic { get; init; }
        }
        public record TaskDefinitionModel
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public int StartEventDefinitionId { get; init; }
            public int EndEventDefinitionId { get; init; }
            public int? CancelEventDefinitionId { get; init; }
            public int ProcessDefinitionId { get; init; }
            public string StartExpression { get; init; }
            public string EndExpression { get; init; }
            public string CancelExpression { get; init; }
            public EventDefinitionModel StartEventDefinition { get; init; }
            public EventDefinitionModel EndEventDefinition { get; init; }
            public EventDefinitionModel CancelEventDefinition { get; init; }
            public string GroupAllocationExpression { get; init; }
            public string UserAllocationExpression { get; init; }
            public bool? AutomaticStart { get; init; }
        }
    }
}