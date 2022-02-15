using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Definition.Application.Queries
{
    public class GetApiVersion
    {
        public record Query : IRequest<Model>;

        public record Model
        {
            public string Version { get; init; }
            public DateTime Date { get; init; }
        }

        public class QueryHandler : IRequestHandler<Query, Model>
        {
            public Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                var version = Environment.GetEnvironmentVariable("APP_VERSION");
                var date = Environment.GetEnvironmentVariable("APP_DATE");
                return Task.FromResult(DateTime.TryParse(date, out var buildDate)
                    ? new Model { Version = version, Date = buildDate }
                    : new Model { Version = version, Date = DateTime.MinValue });
            }
        }
    }
}