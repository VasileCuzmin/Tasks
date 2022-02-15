namespace Tasks.Definition.Domain.Entities
{
    public class TaskDefinition
    {
        public int TaskDefinitionId { get; set; }
        public string Name { get; set; }
        public int StartEventDefinitionId { get; set; }
        public int EndEventDefinitionId { get; set; }
        public int? CancelEventDefinitionId { get; set; }
        public int ProcessDefinitionId { get; set; }
        public string StartExpression { get; set; }
        public string EndExpression { get; set; }
        public string CancelExpression { get; set; }

        public EventDefinition StartEventDefinition { get; set; }
        public EventDefinition EndEventDefinition { get; set; }
        public EventDefinition CancelEventDefinition { get; set; }
        public string GroupAllocationExpression { get; set; }
        public string UserAllocationExpression { get; set; }
        public bool? AutomaticStart { get; set; }
    }
}
