using System.Collections.Generic;
using System.Threading.Tasks;
using NBB.Data.Abstractions;
using Tasks.Definition.Domain.Entities;

namespace Tasks.Definition.Domain.Repositories
{
    public interface ITaskDefinitionRepository : ICrudRepository<TaskDefinition>
    {
        Task<bool> Any(int processDefinitionId, int eventDefinitionId);
        Task<IList<TaskDefinition>> GetByProcessId(int processDefinitionId);
        Task<IList<TaskDefinition>> GetByNameAsync(string requestName);
        Task UpdateAll(IList<TaskDefinition> taskDefinitions);
        Task PersistAllAsync(List<TaskDefinition> taskDefinitions, List<TaskDefinition> deleteTasks);
    }
}