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
    public class AddTaskDefinition
    {
        // TODO: why isn't the AutoStart property included?
        public record Command(
            int ProcessDefinitionId,
            string Name,
            int StartEventDefinitionId,
            int EndEventDefinitionId,
            int? CancelEventDefinitionId,
            string StartExpression,
            string EndExpression,
            string CancelExpression,
            string GroupAllocationExpression,
            string UserAllocationExpression
        ) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly IProcessDefinitionRepository _processDefinitionRepository;
            private readonly IProcessEventDefinitionRepository _processEventDefinitionRepository;

            public Validator(IProcessDefinitionRepository processDefinitionRepository, IProcessEventDefinitionRepository processEventDefinitionRepository)
            {
                _processDefinitionRepository = processDefinitionRepository;
                _processEventDefinitionRepository = processEventDefinitionRepository;

                RuleFor(a => a.ProcessDefinitionId)
                    .NotEmpty()
                    .MustAsync(ValidateProcessDefinitionId).WithMessage("Process definition missing");
                RuleFor(a => a.Name).NotEmpty();
                RuleFor(a => a.StartEventDefinitionId)
                    .NotEmpty()
                    .MustAsync(ValidateEventDefinitionId).WithMessage("Start process event definition missing");
                RuleFor(a => a.EndEventDefinitionId)
                    .NotEmpty()
                    .MustAsync(ValidateEventDefinitionId).WithMessage("End process event definition missing"); ;
                RuleFor(a => a.CancelEventDefinitionId)
                    .NotEmpty()
                    .MustAsync(ValidateCancelEventDefinitionId).WithMessage("Cancel process event definition missing"); ;
            }

            private async Task<bool> ValidateProcessDefinitionId(int processDefinitionId, CancellationToken cancellationToken)
            {
                var processDefinition = await _processDefinitionRepository.GetByIdAsync(processDefinitionId, cancellationToken);
                return processDefinition != null;
            }

            private async Task<bool> ValidateEventDefinitionId(Command command, int eventDefinitionId, CancellationToken cancellationToken)
            {
                var eventDefinition = await _processEventDefinitionRepository.GetByIdAsync(new object[] { command.ProcessDefinitionId, eventDefinitionId }, cancellationToken);
                return eventDefinition != null;
            }
            private async Task<bool> ValidateCancelEventDefinitionId(Command command, int? eventDefinitionId, CancellationToken cancellationToken)
            {
                var eventDefinition = await _processEventDefinitionRepository.GetByIdAsync(new object[] { command.ProcessDefinitionId, eventDefinitionId }, cancellationToken);
                return eventDefinition != null;
            }
        }

        internal class Handler : IRequestHandler<Command>
        {
            private readonly ITaskDefinitionRepository _repository;
            private readonly IMapper _mapper;

            public Handler(ITaskDefinitionRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var taskDefinition = _mapper.Map<TaskDefinition>(request);
                await _repository.AddAsync(taskDefinition, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }
    }
}