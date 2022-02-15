using MediatR;
using NBB.Core.Pipeline;
using NBB.Messaging.Abstractions;
using System;
using System.Collections.Immutable;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Runtime.Application.Commands;

namespace Tasks.Worker.MessagingPipeline
{
    public class ReceiveEventMediatRMiddleware : IPipelineMiddleware<MessagingContext>
    {
        private readonly IMediator _mediator;

        public ReceiveEventMediatRMiddleware(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Invoke(MessagingContext context, CancellationToken cancellationToken, Func<Task> next)
        {
            if (context.MessagingEnvelope.Payload is not ExpandoObject payload)
                throw new Exception("Invalid payload");

            await _mediator.Send(new ReceiveEvent.Command(
                        context.MessagingEnvelope.GetMessageTypeId(),
                        payload.ToImmutableDictionary(),
                        Guid.Parse(context.MessagingEnvelope.Headers[MessagingHeaders.MessageId]),
                        context.MessagingEnvelope.Headers.TryGetValue(MessagingHeaders.Source, out var appName) ? appName : string.Empty
                    ), cancellationToken);

            await next();
        }
    }
}