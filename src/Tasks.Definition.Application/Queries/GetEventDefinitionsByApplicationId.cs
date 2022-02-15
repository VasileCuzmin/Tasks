using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetEventDefinitionsByApplicationId
    {
        public record Query : IRequest<IEnumerable<EventDefinition>>
        {
            public int ApplicationId { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<EventDefinition>>
        {
            private readonly IEventDefinitionRepository _repository;

            public QueryHandler(IEventDefinitionRepository repository)
            {
                _repository = repository;
            }


            public async Task<IEnumerable<EventDefinition>> Handle(Query request, CancellationToken cancellationToken)
            {
                var eventDefinitions = await _repository.GetAllByApplicationIdAsync(request.ApplicationId);
                return eventDefinitions;
            }
        }
    }
}