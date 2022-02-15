using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetApplicationProcessDefinitions
    {
        public record Query : IRequest<IEnumerable<Model>>
        {
            public int ApplicationId { get; init; }
        }

        public record Model
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public string ProcessIdentifierEventProps { get; init; }
            public bool Enabled { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Model>>
        {
            private readonly IProcessDefinitionRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IProcessDefinitionRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                var processes = await _repository.GetAllAsync(request.ApplicationId);
                var result = _mapper.Map<IEnumerable<Model>>(processes);
                return result;
            }
        }
    }
}