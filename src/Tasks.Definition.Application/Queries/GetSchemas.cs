using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;

namespace Tasks.Definition.Application.Queries
{
    public class GetSchemas
    {
        public record Query : IRequest<IEnumerable<Model>>
        {
            public int? StartEventId { get; init; }
            public int? EndEventId { get; init; }
            public int? CancelEventId { get; init; }
            public int ApplicationId { get; init; }
        }

        public record Model
        {
            public string Schema { get; init; }
            public string EventName { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, IEnumerable<Model>>
        {
            private readonly IEventDefinitionRepository _eventDefinitionRepository;

            public QueryHandler(IEventDefinitionRepository eventDefinitionRepository)
            {
                _eventDefinitionRepository = eventDefinitionRepository;
            }

            public async Task<IEnumerable<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                var eventSchemas =
                    await _eventDefinitionRepository.GetSchemasAsync(request.ApplicationId, request.StartEventId.GetValueOrDefault(), request.EndEventId.GetValueOrDefault(), request.CancelEventId.GetValueOrDefault());

                var valueTuples = eventSchemas.ToList();

                return valueTuples.Where(i => !string.IsNullOrEmpty(i.Item1)).Select(i =>
                {
                    var temp = JsonConvert.DeserializeObject(i.Item1) as JObject;
                    if (temp?.Property("Metadata") != null) temp.Property("Metadata").Remove();

                    return new Model { Schema = temp?.ToString(), EventName = i.Item2 };
                });
            }
        }
    }
}