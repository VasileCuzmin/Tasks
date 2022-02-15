using FluentValidation;
using MediatR;
using NBB.Application.DataContracts.Schema;
using NBB.Messaging.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Application.Commands;
using Tasks.PublishedLanguage.Commands;
using Tasks.PublishedLanguage.Events.Definition;

namespace Tasks.Definition.Application.IntegrationEventHandlers
{
    public class LanguagePublishedEventHandler : INotificationHandler<SchemaDefinitionUpdated>
    {
        private readonly IMediator _mediator;
        private readonly IMessageBusPublisher _messageBusPublisher;

        public LanguagePublishedEventHandler(IMediator mediator, IMessageBusPublisher messageBusPublisher)
        {
            _mediator = mediator;
            _messageBusPublisher = messageBusPublisher;
        }

        public async Task Handle(SchemaDefinitionUpdated notification, CancellationToken cancellationToken)
        {
            foreach (var eventDefinition in notification.Definitions)
                await _mediator.Send(new CreateOrUpdateEventDefinition.Command(eventDefinition.Name, eventDefinition.Topic, notification.ApplicationName, eventDefinition.SampleJson), cancellationToken);

            await _messageBusPublisher.PublishAsync(new Shutdown(), cancellationToken);
        }

        public class Validator : AbstractValidator<LanguagePublished>
        {
            public Validator()
            {
                RuleForEach(a => a.SchemaDefinitions).SetValidator(new EventDefinitionValidator());
            }
        }

        public class EventDefinitionValidator : AbstractValidator<SchemaDefinition>
        {
            public EventDefinitionValidator()
            {
                RuleFor(a => a.Name).NotEmpty();
            }
        }
    }
}