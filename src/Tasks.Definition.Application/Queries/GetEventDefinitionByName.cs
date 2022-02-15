using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetEventDefinitionByName
    {
        public record Query : IRequest<EventDefinition>
        {
            public string Name { get; init; }
            public int ApplicationId { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, EventDefinition>
        {
            private readonly IEventDefinitionRepository _repository;

            public QueryHandler(IEventDefinitionRepository repository)
            {
                this._repository = repository;
            }

            public async Task<EventDefinition> Handle(Query request, CancellationToken cancellationToken)
            {
                var eventDefinition = await _repository.GetAsync(request.ApplicationId, request.Name);
                return eventDefinition;
            }
        }
    }
}