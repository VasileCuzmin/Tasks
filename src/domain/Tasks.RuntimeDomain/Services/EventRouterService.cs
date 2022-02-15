using NBB.Data.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.Runtime.Domain.Services
{
    public class EventRouterService
    {
        private readonly IEventDefinitionRepository _eventDefinitionRepository;
        private readonly IProcessDefinitionRepository _processDefinitionRepository;
        private readonly IEventSourcedRepository<ProcessObserver> _processObserverRepository;

        public EventRouterService(
            IEventDefinitionRepository eventDefinitionRepository,
            IEventSourcedRepository<ProcessObserver> processObserverRepository,
            IProcessDefinitionRepository processDefinitionRepository)
        {
            _eventDefinitionRepository = eventDefinitionRepository;
            _processObserverRepository = processObserverRepository;
            _processDefinitionRepository = processDefinitionRepository;
        }

        public async Task<List<ProcessObserver>> GetObserversForEvent(DynamicEvent @event, CancellationToken cancellationToken)
        {
            var observers = new List<ProcessObserver>();

            var eventDefinition = await _eventDefinitionRepository.GetByNameAndApplication(@event.EventDefinitionName, @event.ApplicationName, cancellationToken);

            foreach (var processDefinitionItem in eventDefinition.ProcessDefinitionsDictionary)
            {
                var processId = GetProcessIdentityFor(@event, processDefinitionItem.Value);
                var processObserverId = new ProcessObserverId(processDefinitionItem.Key, processId);

                var processObserver = await _processObserverRepository.GetByIdAsync(processObserverId, cancellationToken);

                if (processObserver == null)
                {
                    var processDefinition = await _processDefinitionRepository.GetById(processDefinitionItem.Key, cancellationToken);
                    processObserver = ProcessObserver.New(processObserverId, processDefinition.TaskDefinitions);
                }

                observers.Add(processObserver);
            }

            return observers;
        }

        public async Task<List<ProcessObserver>> GetObserversForProcess(int processDefinitionId, ProcessId processId, CancellationToken cancellationToken)
        {
            var observers = new List<ProcessObserver>();

            var pd = new ProcessDefinitionId(processDefinitionId);
            var processObserverId = new ProcessObserverId(pd, processId);

            var processObserver = await _processObserverRepository.GetByIdAsync(processObserverId, cancellationToken);

            if (processObserver == null)
            {
                var processDefinition = await _processDefinitionRepository.GetById(pd, cancellationToken);
                processObserver = ProcessObserver.New(processObserverId, processDefinition.TaskDefinitions);
            }

            observers.Add(processObserver);
            return observers;
        }

        private ProcessId GetProcessIdentityFor(DynamicEvent @event, IdentifierPropsMap identifierPropsMap)
        {
            var keyValues =
                 identifierPropsMap.Map.Select(keyValuePair
                 => new ImmutableKeyValue(keyValuePair.Key, @event.Payload.TryGetValue(keyValuePair.Value, out var value) ? value?.ToString() : null));

            return new ProcessId(keyValues);
        }
    }
}