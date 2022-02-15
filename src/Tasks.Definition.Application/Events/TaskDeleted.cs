using MediatR;

namespace Tasks.Definition.Application.Events
{
    public record TaskDeleted(int Id) : INotification;
}