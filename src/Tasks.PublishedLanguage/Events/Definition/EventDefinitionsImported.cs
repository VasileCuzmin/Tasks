namespace Tasks.PublishedLanguage.Events.Definition
{
    public record EventDefinitionsImported : TasksEventBase
    {
        public string ApplicationName { get; init; }
    }
}
