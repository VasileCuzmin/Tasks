using System.Collections.Generic;
using System.Threading.Tasks;
using NBB.Data.Abstractions;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Definition.Domain.Repositories
{
    public interface IProcessDefinitionRepository : ICrudRepository<ProcessDefinition>
    {        
        Task<ProcessDefinition> GetAsync(int applicationId, string processDefinitionName);
        Task<IEnumerable<ProcessDefinition>> GetAllAsync(int applicationId);
        Task UpdateEnabled(int id, bool enabled);
        Task<List<string>> GetAllTopicsForEnabledProcesses();
    }
}