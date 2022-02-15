using FluentValidation;
using MediatR;
using NBB.Data.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Application.Events;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class RemoveTaskDefinition
    {
        public record Command(int Id) : IRequest;

        public class Validator : AbstractValidator<Command>
        {
            private readonly ITaskDefinitionRepository _taskDefinitionRepository;

            public Validator(ITaskDefinitionRepository taskDefinitionRepository)
            {
                _taskDefinitionRepository = taskDefinitionRepository;

                RuleFor(a => a.Id)
                    .NotEmpty()
                    .MustAsync(ValidateTaskDefinitionId).WithMessage("Task definition missing");
            }

            private async Task<bool> ValidateTaskDefinitionId(int taskDefinitionId, CancellationToken cancellationToken)
            {
                var taskDefinition = await _taskDefinitionRepository.GetByIdAsync(taskDefinitionId, cancellationToken);
                return taskDefinition != null;
            }
        }

        internal class Handler : IRequestHandler<Command>
        {
            private readonly ITaskDefinitionRepository _repository;
            private readonly IMediator _mediator;

            public Handler(ITaskDefinitionRepository repository, IMediator mediator)
            {
                _repository = repository;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                await _repository.Remove(request.Id, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken: cancellationToken);
                await _mediator.Publish(new TaskDeleted(request.Id), cancellationToken);

                return Unit.Value;
            }
        }
    }
}