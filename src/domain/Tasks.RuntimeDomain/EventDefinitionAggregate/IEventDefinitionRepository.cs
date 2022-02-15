using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Runtime.Domain.EventDefinitionAggregate
{
    public interface IEventDefinitionRepository
    {
        Task<EventDefinition> GetByNameAndApplication(EventDefinitionName eventDefinitionName, string applicationName, CancellationToken cancellationToken);
    }
}
