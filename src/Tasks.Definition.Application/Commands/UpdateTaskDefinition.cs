using AutoMapper;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Application.Events;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Commands
{
    public class UpdateTaskDefinition
    {
        public record Command(List<Command.TaskModel> Tasks, List<Command.DeleteTaskModel> DeleteTasks, int ProcessDefinitionId) : IRequest
        {
            public record TaskModel
            {
                public int ProcessDefinitionId { get; init; }
                public int Id { get; init; }
                public string Name { get; init; }
                public int StartEventDefinitionId { get; init; }
                public int EndEventDefinitionId { get; init; }
                public int? CancelEventDefinitionId { get; init; }
                public string StartExpression { get; init; }
                public string EndExpression { get; init; }
                public string CancelExpression { get; init; }
                public string GroupAllocationExpression { get; init; }
                public string UserAllocationExpression { get; init; }
                public bool? AutomaticStart { get; init; }
            }

            public class DeleteTaskModel
            {
                public int TaskId { get; set; }
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(IValidator<Command.TaskModel> taskModelValidator)
            {
                RuleFor(t => t.Tasks).NotEmpty().When(t => !t.DeleteTasks.Any());
                RuleForEach(t => t.Tasks).SetValidator(taskModelValidator).When(t => !t.DeleteTasks.Any());
            }
        }

        public class TaskModelValidator : AbstractValidator<Command.TaskModel>
        {
            public TaskModelValidator()
            {
                RuleFor(t => t.Name).NotEmpty();
                RuleFor(t => t.ProcessDefinitionId).NotEmpty();
                RuleFor(t => t.StartEventDefinitionId).NotEmpty();
                RuleFor(t => t.EndEventDefinitionId).NotEmpty();
            }
        }

        internal class Handler : IRequestHandler<Command>
        {
            private readonly ITaskDefinitionRepository _repository;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;

            public Handler(ITaskDefinitionRepository repository, IMapper mapper, IMediator mediator)
            {
                _repository = repository;
                _mapper = mapper;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var taskDefinitions = _mapper.Map<List<TaskDefinition>>(request.Tasks);
                var deleteTasks = _mapper.Map<List<TaskDefinition>>(request.DeleteTasks);

                await _repository.PersistAllAsync(taskDefinitions, deleteTasks);

                var updatedTaskDefinitions = await _repository.GetByProcessId(request.ProcessDefinitionId);
                var taskDefinitionsUpdated = _mapper.Map<List<TaskUpdated.TaskDefinitionModel>>(updatedTaskDefinitions);

                await _mediator.Publish(new TaskUpdated(taskDefinitionsUpdated), cancellationToken);

                return Unit.Value;
            }
        }
    }
}