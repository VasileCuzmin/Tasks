using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetTaskDefinitionsByProcessId
    {
        public record Query : IRequest<IList<TaskModel>>
        {
            public int ProcessId { get; init; }
        }
        public record TaskModel
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public int StartEventDefinitionId { get; init; }
            public int EndEventDefinitionId { get; init; }
            public int? CancelEventDefinitionId { get; init; }
            public int ProcessDefinitionId { get; init; }
            public string StartExpression { get; init; }
            public string EndExpression { get; init; }
            public string CancelExpression { get; init; }
            public string GroupAllocationExpression { get; init; }
            public string UserAllocationExpression { get; init; }
            public bool? AutomaticStart { get; init; }
        }
        public class QueryHandler : IRequestHandler<Query, IList<TaskModel>>
        {
            private readonly ITaskDefinitionRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(ITaskDefinitionRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<IList<TaskModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                var taskDefinitions = await _repository.GetByProcessId(request.ProcessId);
                var result = _mapper.Map<IList<TaskModel>>(taskDefinitions);
                return result;
            }
        }
    }
}
