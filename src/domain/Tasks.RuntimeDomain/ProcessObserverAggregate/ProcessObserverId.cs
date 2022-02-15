using Tasks.Runtime.Domain.ProcessDefinitionAggregate;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate
{
    public record ProcessObserverId(ProcessDefinitionId ProcessDefinitionId, ProcessId ProcessId)
    {
        public override string ToString()
        {
            return $"ProcessDefinitionId:{ProcessDefinitionId}_{ProcessId}";
        }
    }
}
