namespace Tasks.Definition.Domain.Entities
{
    public class EventDefinition
    {
        public int EventDefinitionId { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public int ApplicationId { get; set; }
        public string Schema { get; set; }
        public Application Application { get; set; }
    }
}