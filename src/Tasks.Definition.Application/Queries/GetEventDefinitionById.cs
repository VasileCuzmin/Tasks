using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetEventDefinitionById
    {
        public record Query : IRequest<EventDefinition>
        {
            public int EventDefinitionId { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, EventDefinition>
        {
            private readonly IEventDefinitionRepository _repository;

            public QueryHandler(IEventDefinitionRepository repository)
            {
                _repository = repository;
            }


            public async Task<EventDefinition> Handle(Query request, CancellationToken cancellationToken)
            {
                var eventDefinition = await _repository.GetByIdAsync(request.EventDefinitionId, cancellationToken, "Application");
                return eventDefinition;
            }
        }
    }
}