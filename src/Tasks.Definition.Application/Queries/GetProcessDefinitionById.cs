using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetProcessDefinitionById
    {
        public record Query : IRequest<Model>
        {
            public int ProcessDefinitionId { get; init; }           
        }

        public record ApplicationModel
        {
            public int Id { get; init; }
            public string Name { get; init; }
        }

        public record EventDefinitionModel
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public string Topic { get; init; }
        }

        public record TaskDefinitionModel
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

            public EventDefinitionModel StartEventDefinition { get; init; }
            public EventDefinitionModel EndEventDefinition { get; init; }
            public EventDefinitionModel CancelEventDefinition { get; init; }
            public string GroupAllocationExpression { get; init; }
            public string UserAllocationExpression { get; init; }
            public bool? AutomaticStart { get; init; }
        }

        public record ProcessEventDefinitionModel
        {
            public int ProcessDefinitionId { get; init; }
            public string ProcessIdentifierProps { get; init; }
            public EventDefinitionModel EventDefinition { get; init; }
        }

        public record Model
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public string ProcessIdentifierEventProps { get; init; }
            public int ApplicationId { get; init; }
            public bool Enabled { get; init; }

            public List<ProcessEventDefinitionModel> ProcessEventDefinitions { get; init; }
            public List<TaskDefinitionModel> TaskDefinitions { get; init; }
            public ApplicationModel Application { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly IProcessDefinitionRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IProcessDefinitionRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                var item = await _repository.GetByIdAsync(request.ProcessDefinitionId, cancellationToken, new string[] { "ProcessEventDefinitions", "ProcessEventDefinitions.EventDefinition", "TaskDefinitions", "Application" });
                var result = _mapper.Map<Model>(item);
                return result;
            }
        }
    }
}