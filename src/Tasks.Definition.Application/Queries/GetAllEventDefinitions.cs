using MediatR;
using NBB.Core.Abstractions.Paging;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetAllEventDefinitions
    {
        public record Query : IRequest<PagedResult<EventDefinition>>
        {
            protected internal const int DefaultPageSize = 20;
            public int Page { get; init; } = 1;
            public int PageSize { get; init; } = DefaultPageSize;
        }

        public class QueryHandler : IRequestHandler<Query, PagedResult<EventDefinition>>
        {
            private readonly IEventDefinitionRepository _repository;

            public QueryHandler(IEventDefinitionRepository repository)
            {
                _repository = repository;
            }

            public async Task<PagedResult<EventDefinition>> Handle(Query request, CancellationToken cancellationToken)
            {
                var pageRequest = new PageRequest(request.Page <= 0 ? 1 : request.Page, request.PageSize <= 0 ? Query.DefaultPageSize : request.PageSize);
                var result = await _repository.GetAllPagedAsync(pageRequest, cancellationToken, new string[] { "Application" });
                return result;

            }
        }
    }
}