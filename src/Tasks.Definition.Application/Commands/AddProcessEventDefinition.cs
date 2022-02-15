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
    public class AddProcessEventDefinition
    {
        public record Command(int ProcessDefinitionId, int EventDefinitionId, string ProcessIdentifierProps) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly IProcessDefinitionRepository _processDefinitionRepository;
            private readonly IEventDefinitionRepository _eventDefinitionRepository;
            private readonly IProcessEventDefinitionRepository _processEventDefinitionRepository;

            public Validator(IProcessDefinitionRepository processDefinitionRepository, IEventDefinitionRepository eventDefinitionRepository, IProcessEventDefinitionRepository processEventDefinitionRepository)
            {
                _processDefinitionRepository = processDefinitionRepository;
                _eventDefinitionRepository = eventDefinitionRepository;
                _processEventDefinitionRepository = processEventDefinitionRepository;

                RuleFor(a => a.ProcessDefinitionId)
                    .NotEmpty()
                    .MustAsync(ValidateProcessDefinitionId).WithMessage("Process definition missing");
                RuleFor(a => a.EventDefinitionId)
                    .NotEmpty()
                    .MustAsync(ValidateEventDefinitionId).WithMessage("Event definition missing");
                RuleFor(a => a)
                    .MustAsync(ValidateDuplicateProcessEventDefinition).WithMessage("Duplicate process event definition")
                    .MustAsync(ValidateApplication).WithMessage("Process and event definition should have the same application");
                RuleFor(a => a.ProcessIdentifierProps).NotEmpty();
            }

            private async Task<bool> ValidateProcessDefinitionId(int processDefinitionId, CancellationToken cancellationToken)
            {
                var processDefinition = await _processDefinitionRepository.GetByIdAsync(processDefinitionId, cancellationToken);
                return processDefinition != null;
            }

            private async Task<bool> ValidateEventDefinitionId(int eventDefinitionId, CancellationToken cancellationToken)
            {
                var eventDefinition = await _eventDefinitionRepository.GetByIdAsync(eventDefinitionId, cancellationToken);
                return eventDefinition != null;
            }

            private async Task<bool> ValidateDuplicateProcessEventDefinition(Command command, CancellationToken cancellationToken)
            {
                var processDefinition = await _processEventDefinitionRepository.GetByIdAsync(new object[] { command.ProcessDefinitionId, command.EventDefinitionId }, cancellationToken);
                return processDefinition == null;
            }

            private async Task<bool> ValidateApplication(Command command, CancellationToken cancellationToken)
            {
                var processDefinition = await _processDefinitionRepository.GetByIdAsync(command.ProcessDefinitionId, cancellationToken);
                var eventDefinition = await _eventDefinitionRepository.GetByIdAsync(command.EventDefinitionId, cancellationToken);

                return processDefinition.ApplicationId == eventDefinition.ApplicationId;
            }
        }

        internal class Handler : IRequestHandler<Command>
        {
            private readonly IProcessEventDefinitionRepository _repository;
            private readonly IMapper _mapper;

            public Handler(IProcessEventDefinitionRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var processEventDefinition = _mapper.Map<ProcessEventDefinition>(request);
                await _repository.AddAsync(processEventDefinition, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }
    }
}