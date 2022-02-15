using MediatR;

namespace Tasks.Definition.Application.Events
{
    public record ProcessUpdated(ProcessUpdated.Model ProcessDefinition) : INotification
    {
        public record Model
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public string ProcessIdentifierEventProps { get; init; }
            public int ApplicationId { get; init; }
            public bool Enabled { get; init; }
        }
    }
}