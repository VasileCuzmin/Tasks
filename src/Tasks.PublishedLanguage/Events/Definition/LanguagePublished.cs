using NBB.Application.DataContracts.Schema;
using System.Collections.Generic;

namespace Tasks.PublishedLanguage.Events.Definition
{
    public record LanguagePublished : TasksEventBase
    {
        public string ApplicationName { get; init; }
        public List<SchemaDefinition> SchemaDefinitions { get; init; }
    }
}