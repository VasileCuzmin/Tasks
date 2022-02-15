using AutoMapper;
using MediatR;
using NBB.Core.Abstractions.Paging;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetAllApplications
    {
        public record Query : IRequest<PagedResult<Model>>
        {
            protected internal const int DefaultPageSize = 100;
            public int Page { get; init; } = 1;
            public int PageSize { get; init; } = DefaultPageSize;
        }

        public record Model
        {
            public int Id { get; init; }
            public string Name { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, PagedResult<Model>>
        {

            private readonly IApplicationRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IApplicationRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<PagedResult<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                var pageRequest = new PageRequest(request.Page <= 0 ? 1 : request.Page, request.PageSize <= 0 ? Query.DefaultPageSize : request.PageSize);
                var paged = await _repository.GetAllPagedAsync(pageRequest, cancellationToken);
                var result = paged.Map(x => _mapper.Map<Model>(x));
                return result;
            }
        }
    }
}