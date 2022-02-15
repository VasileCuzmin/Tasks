using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetApplicationById
    {
        public record Query : IRequest<Domain.Entities.Application>
        {
            public int ApplicationId { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, Domain.Entities.Application>
        {
            private readonly IApplicationRepository _repository;

            public QueryHandler(IApplicationRepository repository)
            {
                _repository = repository;
            }


            public async Task<Domain.Entities.Application> Handle(Query request, CancellationToken cancellationToken)
            {
                var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);
                return application;
            }
        }
    }
}