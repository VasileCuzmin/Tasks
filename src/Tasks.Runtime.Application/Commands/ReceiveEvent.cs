using MediatR;
using NBB.Data.Abstractions;
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.Services;

namespace Tasks.Runtime.Application.Commands
{
    public class ReceiveEvent
    {
        public record Command(string EventType, ImmutableDictionary<string, object> Payload, Guid MessageId, string ApplicationName) : IRequest;

        internal class Handler : IRequestHandler<Command>
        {
            private readonly EventRouterService _eventRouterService;
            private readonly IEventSourcedRepository<ProcessObserver> _processObserverRepository;
            private readonly IExpressionEvaluationService _expressionEvaluationService;

            public Handler(EventRouterService eventRouterService, IEventSourcedRepository<ProcessObserver> processObserverRepository, IExpressionEvaluationService expressionEvaluationService)
            {
                _eventRouterService = eventRouterService;
                _processObserverRepository = processObserverRepository;
                _expressionEvaluationService = expressionEvaluationService;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
               var @event = new DynamicEvent(new EventDefinitionName(request.EventType), request.Payload, request.MessageId, request.ApplicationName);
                var observers = await _eventRouterService.GetObserversForEvent(@event, cancellationToken);
                foreach (var observer in observers)
                {
                    observer.ObserveEvent(@event, _expressionEvaluationService);
                    await _processObserverRepository.SaveAsync(observer, cancellationToken);
                }
                
                return Unit.Value;
            }
        }
    }
}