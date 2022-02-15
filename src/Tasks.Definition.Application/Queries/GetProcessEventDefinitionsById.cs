using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetProcessEventDefinitionsById
    {
        public record Query : IRequest<ProcessEventDefinition>
        {
            public int ProcessDefinitionId { get; init; }
            public int EventDefinitionId { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, ProcessEventDefinition>
        {
            private readonly IProcessEventDefinitionRepository _repository;

            public QueryHandler(IProcessEventDefinitionRepository repository)
            {
                _repository = repository;
            }

            public async Task<ProcessEventDefinition> Handle(Query request, CancellationToken cancellationToken)
            {
                var item = await _repository.GetByIdAsync(new object[] { request.ProcessDefinitionId, request.EventDefinitionId }, cancellationToken);
                return item;
            }
        }
    }
}