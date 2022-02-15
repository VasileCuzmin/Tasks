using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class CreateOrUpdateEventDefinition
    {
        public record Command(string Name, string Topic, string ApplicationName, string Schema) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(a => a.ApplicationName).NotEmpty();
                RuleFor(a => a.Name).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IEventDefinitionRepository _eventDefinitionRepository;
            private readonly IApplicationRepository _applicationRepository;
            private readonly IMediator _mediator;

            public Handler(IEventDefinitionRepository eventDefinitionRepository, IApplicationRepository applicationRepository, IMediator mediator)
            {
                _eventDefinitionRepository = eventDefinitionRepository;
                _applicationRepository = applicationRepository;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var application = await _applicationRepository.GetByNameAsync(request.ApplicationName);

                if (application == null)
                {
                    await _mediator.Send(new CreateApplication.Command(request.ApplicationName), cancellationToken);

                    application = await _applicationRepository.GetByNameAsync(request.ApplicationName);
                }

                var eventDefinition = await _eventDefinitionRepository.GetAsync(application.ApplicationId, request.Name);
                if (eventDefinition == null)
                {
                    await _mediator.Send(
                        new CreateEventDefinition.Command(request.Name, request.Topic, application.ApplicationId, request.Schema), cancellationToken);
                }
                else
                {
                    await _mediator.Send(
                        new UpdateEventDefinition.Command(eventDefinition.EventDefinitionId, request.Name, request.Topic, application.ApplicationId, request.Schema),
                        cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}