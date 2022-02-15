using MediatR;

namespace Tasks.Definition.Application.Events
{
    public record ApplicationUpdated(string ApplicationName, int Id) : INotification;
}
