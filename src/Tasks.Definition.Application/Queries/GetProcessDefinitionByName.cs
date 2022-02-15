using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetProcessDefinitionByName
    {
        public record Query : IRequest<ProcessDefinition>
        {
            public string Name { get; init; }
            public int ApplicationId { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, ProcessDefinition>
        {
            private readonly IProcessDefinitionRepository _repository;

            public QueryHandler(IProcessDefinitionRepository repository)
            {
                this._repository = repository;
            }

            public async Task<ProcessDefinition> Handle(Query request, CancellationToken cancellationToken)
            {
                var item = await _repository.GetAsync(request.ApplicationId, request.Name);
                return item;
            }
        }
    }
}