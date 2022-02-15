using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using NBB.Messaging.Abstractions;
using Tasks.PublishedLanguage.Commands;
using Tasks.PublishedLanguage.Events.Definition;

namespace Tasks.Runtime.Application.CommandHandlers
{
    class ShutdownCommandHandler : IRequestHandler<Shutdown>
    {
        private readonly ILogger<Shutdown> _logger;
        private readonly IMessageBusPublisher _messageBusPublisher;

        public ShutdownCommandHandler(ILogger<Shutdown> logger, IMessageBusPublisher messageBusPublisher)
        {
            _logger = logger;
            _messageBusPublisher = messageBusPublisher;
        }

        public Task<Unit> Handle(Shutdown request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("The process will shutdown in 1s");
            Task.Run(async () =>
           {
               await Task.Delay(1000, cancellationToken);
               await _messageBusPublisher.PublishAsync(new TasksWorkerWasRestarted(), cancellationToken);
               Environment.Exit(1);
           }, cancellationToken);

            return Unit.Task;
        }
    }
}