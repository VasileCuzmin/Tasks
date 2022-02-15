using AutoMapper;
using FluentValidation;
using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class CreateEventDefinition
    {
        public record Command(string Name, string Topic, int ApplicationId, string Schema) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly IEventDefinitionRepository _eventDefinitionRepository;
            private readonly IApplicationRepository _applicationRepository;

            public Validator(IEventDefinitionRepository eventDefinitionRepository, IApplicationRepository applicationRepository)
            {
                _eventDefinitionRepository = eventDefinitionRepository;
                _applicationRepository = applicationRepository;

                RuleFor(a => a.ApplicationId)
                    .NotEmpty()
                    .MustAsync(ValidateApplicationId).WithMessage("Application is missing");
                RuleFor(a => a.Name)
                    .NotEmpty()
                    .MustAsync(ValidateName).WithMessage("Event definition already exists");
            }

            private async Task<bool> ValidateApplicationId(int applicationId, CancellationToken cancellationToken)
            {
                var application = await _applicationRepository.GetByIdAsync(applicationId, cancellationToken);
                return application != null;
            }

            private async Task<bool> ValidateName(Command command, string eventDefinitionName, CancellationToken cancellationToken)
            {
                var eventDefinition = await _eventDefinitionRepository.GetAsync(command.ApplicationId, eventDefinitionName);
                return eventDefinition == null;
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IEventDefinitionRepository _eventDefinitionRepository;
            private readonly IMapper _mapper;

            public Handler(IEventDefinitionRepository eventDefinitionRepository, IMapper mapper)
            {
                _eventDefinitionRepository = eventDefinitionRepository;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var eventDefinition = _mapper.Map<EventDefinition>(request);
                await _eventDefinitionRepository.AddAsync(eventDefinition, cancellationToken);
                await _eventDefinitionRepository.SaveChangesAsync(cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }
    }
}
