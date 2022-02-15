using NBB.Domain;

namespace Tasks.Runtime.Domain.EventDefinitionAggregate
{
    public record EventDefinitionName(string Value) : Identity<string>(Value);
}
