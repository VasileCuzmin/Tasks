using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Runtime.Domain.ProcessDefinitionAggregate
{
    public interface IProcessDefinitionRepository
    {
        Task<ProcessDefinition> GetById(ProcessDefinitionId processDefinitionId, CancellationToken cancellationToken);
    }
}
