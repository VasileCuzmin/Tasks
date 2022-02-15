using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetTaskDefinitionsByName
    {
        public record Query : IRequest<IList<TaskDefinition>>
        {
            public string Name { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, IList<TaskDefinition>>
        {
            private readonly ITaskDefinitionRepository _repository;

            public QueryHandler(ITaskDefinitionRepository repository)
            {
                _repository = repository;
            }

            public async Task<IList<TaskDefinition>> Handle(Query request, CancellationToken cancellationToken)
            {
                var items = await _repository.GetByNameAsync(request.Name);
                return items;
            }
        }
    }
}