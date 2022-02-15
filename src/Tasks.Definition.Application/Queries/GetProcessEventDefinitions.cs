using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetProcessEventDefinitions
    {
        public record Query : IRequest<IEnumerable<ProcessEventDefinition>>
        {
            public int ProcessDefinitionId { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<ProcessEventDefinition>>
        {
            private readonly IEventDefinitionRepository _repository;

            public QueryHandler(IEventDefinitionRepository repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<ProcessEventDefinition>> Handle(Query request, CancellationToken cancellationToken)
            {
                var eventDefinitionsByProcessId = await _repository.GetAllByProcessIdAsync(request.ProcessDefinitionId);
                return eventDefinitionsByProcessId;
            }
        }
    }
}