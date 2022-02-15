using FluentValidation;
using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class EnableProcessDefinition
    {
        public record Command(int ProcessDefinitionId) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly IProcessDefinitionRepository _processDefinitionRepository;

            public Validator(IProcessDefinitionRepository processDefinitionRepository)
            {
                _processDefinitionRepository = processDefinitionRepository;

                RuleFor(a => a.ProcessDefinitionId)
                    .NotEmpty()
                    .MustAsync(ValidateProcessDefinitionId).WithMessage("Process definition is missing");
            }

            private async Task<bool> ValidateProcessDefinitionId(int processDefinitionId, CancellationToken cancellationToken)
            {
                var processDefinition = await _processDefinitionRepository.GetByIdAsync(processDefinitionId, cancellationToken);
                return processDefinition != null;
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IProcessDefinitionRepository _processDefinitionRepository;

            public Handler(IProcessDefinitionRepository processDefinitionRepository)
            {
                _processDefinitionRepository = processDefinitionRepository;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                await _processDefinitionRepository.UpdateEnabled(request.ProcessDefinitionId, true);
                await _processDefinitionRepository.SaveChangesAsync(cancellationToken: cancellationToken);

                return Unit.Value;
            }
        }
    }
}