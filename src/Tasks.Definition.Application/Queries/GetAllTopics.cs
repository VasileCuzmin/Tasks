using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetAllTopics
    {
        public record Query : IRequest<List<string>>;

        public class QueryHandler : IRequestHandler<Query, List<string>>
        {
            private readonly IEventDefinitionRepository _repository;

            public QueryHandler(IEventDefinitionRepository repository)
            {
                _repository = repository;
            }


            public async Task<List<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                var topics = await _repository.GetAllTopicsAsync();
                return topics;
            }
        }
    }
}