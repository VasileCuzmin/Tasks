using AutoMapper;
using FluentValidation;
using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class UpdateEventDefinition
    {
        public record Command(int EventDefinitionId, string Name, string Topic, int ApplicationId, string Schema) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly IEventDefinitionRepository _eventDefinitionRepository;
            private readonly IApplicationRepository _applicationRepository;

            public Validator(IEventDefinitionRepository eventDefinitionRepository, IApplicationRepository applicationRepository)
            {
                _eventDefinitionRepository = eventDefinitionRepository;
                _applicationRepository = applicationRepository;

                RuleFor(a => a.EventDefinitionId)
                    .NotEmpty()
                    .MustAsync(ValidateEventDefinitionId).WithMessage("Event definition is missing");
                RuleFor(a => a.ApplicationId)
                    .NotEmpty()
                    .MustAsync(ValidateApplicationId).WithMessage("Application is missing");
                RuleFor(a => a.Name)
                    .NotEmpty()
                    .MustAsync(ValidateName).WithMessage("Duplicate event definition");
            }

            private async Task<bool> ValidateEventDefinitionId(int eventDefinitionId, CancellationToken cancellationToken)
            {
                var eventDefinition = await _eventDefinitionRepository.GetByIdAsync(eventDefinitionId, cancellationToken);
                return eventDefinition != null;
            }

            private async Task<bool> ValidateApplicationId(int applicationId, CancellationToken cancellationToken)
            {
                var application = await _applicationRepository.GetByIdAsync(applicationId, cancellationToken);
                return application != null;
            }

            private async Task<bool> ValidateName(Command command, string eventDefinitionName, CancellationToken cancellationToken)
            {
                var eventDefinition = await _eventDefinitionRepository.GetAsync(command.ApplicationId, eventDefinitionName);
                return eventDefinition == null || eventDefinition.EventDefinitionId == command.EventDefinitionId;
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IEventDefinitionRepository _repository;
            private readonly IMapper _mapper;

            public Handler(IEventDefinitionRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var eventDefinition = await _repository.GetByIdAsync(request.EventDefinitionId, cancellationToken);
                _mapper.Map(request, eventDefinition);
                await _repository.Update(eventDefinition, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }
    }
}