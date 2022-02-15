using System.Collections.Generic;

namespace Tasks.Definition.Domain.Entities
{
    public class ProcessDefinition
    {
        public int ProcessDefinitionId { get; set; }
        public string Name { get; set; }
        public string ProcessIdentifierEventProps { get; set; }
        public int ApplicationId { get; set; }
        public bool Enabled { get; set; }

        public List<ProcessEventDefinition> ProcessEventDefinitions { get; set; }
        public List<TaskDefinition> TaskDefinitions { get; set; }
        public Application Application { get; set; }
    }
}