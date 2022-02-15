using Newtonsoft.Json;

namespace Tasks.Definition.Domain.Entities
{
    public class ProcessEventDefinition
    {
        public int ProcessDefinitionId { get; set; }
        public int EventDefinitionId { get; set; }
        public string ProcessIdentifierProps { get; set; }
        [JsonIgnore]
        public ProcessDefinition ProcessDefinition { get; set; }
        public EventDefinition EventDefinition { get; set; }
    }
}
