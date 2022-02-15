using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetTaskDefinitionsById
    {
        public record Query : IRequest<TaskDefinition>
        {
            public int TaskDefinitionId { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, TaskDefinition>
        {
            private readonly ITaskDefinitionRepository _repository;

            public QueryHandler(ITaskDefinitionRepository repository)
            {
                _repository = repository;
            }

            public async Task<TaskDefinition> Handle(Query request, CancellationToken cancellationToken)
            {
                var item = await _repository.GetByIdAsync(request.TaskDefinitionId, cancellationToken,
                    new string[] { "StartEventDefinition", "EndEventDefinition", "CancelEventDefinition" });
                return item;
            }
        }
    }
}