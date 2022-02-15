using NBB.Domain;

namespace Tasks.Runtime.Domain.ProcessDefinitionAggregate
{
    public record AutomaticStart(bool? Value) : Identity<bool?>(Value);
}