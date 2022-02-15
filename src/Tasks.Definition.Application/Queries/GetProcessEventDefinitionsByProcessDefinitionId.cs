using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetProcessEventDefinitionsByProcessDefinitionId
    {
        public record Query : IRequest<IEnumerable<ProcessEventDefinitionModel>>
        {
            public int ProcessDefinitionId { get; init; }
        }

        public class ProcessEventDefinitionModel
        {
            public int ProcessDefinitionId { get; set; }
            public string ProcessIdentifierProps { get; set; }
            public EventDefinitionModel EventDefinition { get; set; }
        }

        public class EventDefinitionModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Topic { get; set; }
            public int ApplicationId { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<ProcessEventDefinitionModel>>
        {
            private readonly IProcessEventDefinitionRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IProcessEventDefinitionRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<IEnumerable<ProcessEventDefinitionModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                var items = await _repository.GetProcessEventDefinitionsByProcesIdAsync(request.ProcessDefinitionId);
                var sortedItems = items.OrderBy(x => x.EventDefinition.Name).ToList();
                var result = _mapper.Map<IEnumerable<ProcessEventDefinitionModel>>(sortedItems);
                return result.ToList();
            }
        }
    }
}