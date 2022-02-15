using System.Collections.Generic;
using System.Threading.Tasks;
using NBB.Data.Abstractions;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Definition.Domain.Repositories
{
    public interface IProcessEventDefinitionRepository : ICrudRepository<ProcessEventDefinition>
    {
        Task<IEnumerable<EventDefinition>> GetEventDefinitionsByProcessDefinitionIdAsync(int processDefinitionId);
        Task<IEnumerable<ProcessEventDefinition>> GetProcessEventDefinitionsByProcesIdAsync(int processDefinitionId);
        Task<IEnumerable<ProcessEventDefinition>> GetAsync(string eventName, string applicationName);
        Task PersistAllAsync(List<ProcessEventDefinition> processEventDefinitions, List<ProcessEventDefinition> deleteProcessEventDefinitions);
    }
}
