using AutoMapper;
using FluentValidation;
using MediatR;
using NBB.Data.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Application.Events;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class UpdateProcessDefinition
    {
        public record Command(int Id, string Name, string ProcessIdentifierEventProps, int ApplicationId, bool Enabled) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly IProcessDefinitionRepository _processDefinitionRepository;
            private readonly IApplicationRepository _applicationRepository;
            private readonly IProcessEventDefinitionRepository _processEventDefinitionRepository;

            public Validator(IProcessDefinitionRepository processDefinitionRepository, IApplicationRepository applicationRepository, IProcessEventDefinitionRepository processEventDefinitionRepository)
            {
                _processDefinitionRepository = processDefinitionRepository;
                _applicationRepository = applicationRepository;
                _processEventDefinitionRepository = processEventDefinitionRepository;

                RuleFor(a => a.Id)
                    .NotEmpty()
                    .MustAsync(ValidateProcessDefinitionId).WithMessage("Process definition is missing");
                RuleFor(a => a.ApplicationId)
                    .NotEmpty()
                    .MustAsync(ValidateApplicationId).WithMessage("Application is missing")
                    .MustAsync(ValidateNewApplicationId).WithMessage("There are event definitions with other application");
                RuleFor(a => a.Name)
                    .NotEmpty()
                    .MustAsync(ValidateName).WithMessage("Duplicate process definition");
            }

            private async Task<bool> ValidateNewApplicationId(Command command, int applicationId, CancellationToken cancellationToken)
            {
                var processDefinition = await _processDefinitionRepository.GetByIdAsync(command.Id, cancellationToken);
                if (processDefinition.ApplicationId == applicationId)
                    return true;

                var eventDefinitions =
                    await _processEventDefinitionRepository.GetEventDefinitionsByProcessDefinitionIdAsync(
                        command.Id);
                return eventDefinitions.All(a => a.ApplicationId == applicationId);
            }

            private async Task<bool> ValidateProcessDefinitionId(int processDefinitionId, CancellationToken cancellationToken)
            {
                var processDefinition = await _processDefinitionRepository.GetByIdAsync(processDefinitionId, cancellationToken);
                return processDefinition != null;
            }

            private async Task<bool> ValidateApplicationId(int applicationId, CancellationToken cancellationToken)
            {
                var application = await _applicationRepository.GetByIdAsync(applicationId, cancellationToken);
                return application != null;
            }

            private async Task<bool> ValidateName(Command command, string processDefinitionName, CancellationToken cancellationToken)
            {
                var processDefinition = await _processDefinitionRepository.GetAsync(command.ApplicationId, processDefinitionName);
                return processDefinition == null || processDefinition.ProcessDefinitionId == command.Id;
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IProcessDefinitionRepository _repository;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;

            public Handler(IProcessDefinitionRepository repository, IMapper mapper, IMediator mediator)
            {
                _repository = repository;
                _mapper = mapper;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var processDefinition = _mapper.Map<ProcessDefinition>(request);
                await _repository.Update(processDefinition, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken: cancellationToken);

                var eventData = _mapper.Map<ProcessUpdated.Model>(request);
                await _mediator.Publish(new ProcessUpdated(eventData), cancellationToken);

                return Unit.Value;
            }
        }
    }
}