using FluentValidation;
using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class RemoveProcessEventDefinition
    {
        public record Command(int ProcessDefinitionId, int EventDefinitionId)  : IRequest;
     
        public class Validator : AbstractValidator<Command>
        {            
            private readonly IProcessEventDefinitionRepository _processEventDefinitionRepository;
            private readonly ITaskDefinitionRepository _taskDefinitionRepository;

            public Validator(IProcessEventDefinitionRepository processEventDefinitionRepository, ITaskDefinitionRepository taskDefinitionRepository)
            {
                _processEventDefinitionRepository = processEventDefinitionRepository;
                _taskDefinitionRepository = taskDefinitionRepository;

                RuleFor(a => a.ProcessDefinitionId)
                    .NotEmpty();
                RuleFor(a => a.EventDefinitionId)
                    .NotEmpty();
                RuleFor(a => a)
                    .MustAsync(ValidateProcessEventDefinition).WithMessage(ValidationMessages.MissingProcessEventDefinition);
                RuleFor(a => a)
                    .MustAsync(ValidateIfTasksExistForEventDefinition).WithMessage(ValidationMessages.RemoveProcessEventWithDependentTasks);
            }

            private async Task<bool> ValidateProcessEventDefinition(Command command, CancellationToken cancellationToken)
            {
                var processDefinition = await _processEventDefinitionRepository.GetByIdAsync(new object[] { command.ProcessDefinitionId, command.EventDefinitionId }, cancellationToken);
                return processDefinition != null;
            }

            private async Task<bool> ValidateIfTasksExistForEventDefinition(Command command, CancellationToken cancellationToken)
            {
                var exists = await _taskDefinitionRepository.Any(command.ProcessDefinitionId, command.EventDefinitionId);                
                return !exists;
            }
        }

        internal class Handler : IRequestHandler<Command>
        {
            private readonly IProcessEventDefinitionRepository _repository;

            public Handler(IProcessEventDefinitionRepository repository)
            {
                _repository = repository;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {                
                await _repository.Remove(new object[] { request.ProcessDefinitionId, request.EventDefinitionId }, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }
    }
}