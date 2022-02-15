using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Definition.Domain.Repositories;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using IEventDefinitionRepository = Tasks.Runtime.Domain.EventDefinitionAggregate.IEventDefinitionRepository;

namespace Tasks.Domain.Data.Repositories.Runtime
{
    public class EventDefinitionRepository : IEventDefinitionRepository
    {
        private readonly IProcessEventDefinitionRepository _processEventDefinitionRepository;

        public EventDefinitionRepository(IProcessEventDefinitionRepository processEventDefinitionRepository)
        {
            _processEventDefinitionRepository = processEventDefinitionRepository;
        }

        public async Task<EventDefinition> GetByNameAndApplication(EventDefinitionName eventDefinitionName, string applicationName, CancellationToken cancellationToken)
        {
            //This should be outside.Don't call repo inside another repo
            var processEventDefinitions = await _processEventDefinitionRepository.GetAsync(eventDefinitionName.Value, applicationName);
            var runtimeProcessDefinitions = processEventDefinitions
                .ToDictionary(
                    x => new ProcessDefinitionId(x.ProcessDefinitionId),
                    x => IdentifierPropsMap.From(x.ProcessDefinition.ProcessIdentifierEventProps, x.ProcessIdentifierProps));

            return new EventDefinition(eventDefinitionName,
                runtimeProcessDefinitions);
        }
    }
}