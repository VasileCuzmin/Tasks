using MediatR;
namespace Tasks.PublishedLanguage.Events.Definition
{
    public record TasksWorkerWasRestarted : INotification;
}