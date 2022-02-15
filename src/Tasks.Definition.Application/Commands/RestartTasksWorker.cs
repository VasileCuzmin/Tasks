using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tasks.PublishedLanguage.Commands;

namespace Tasks.Definition.Application.Commands
{
    public class RestartTasksWorker
    {
        public record Command : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                await _mediator.Send(new Shutdown(), cancellationToken);

                return Unit.Value;
            }
        }
    }
}