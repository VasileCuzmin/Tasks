using System.Collections.Generic;
using System.Threading.Tasks;
using NBB.Data.Abstractions;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Definition.Domain.Repositories
{
    public interface IEventDefinitionRepository : ICrudRepository<EventDefinition>
    {
        Task<EventDefinition> GetAsync(int applicationId, string eventDefinitionName);
        Task<IEnumerable<EventDefinition>> GetAllByApplicationIdAsync(int applicationId);
        Task<IEnumerable<ProcessEventDefinition>> GetAllByProcessIdAsync(int processDefinitionId);
        Task<IEnumerable<(string,string)>> GetSchemasAsync(int applicationId, params int[] ids);
        Task<List<string>> GetAllTopicsAsync();
    }
}