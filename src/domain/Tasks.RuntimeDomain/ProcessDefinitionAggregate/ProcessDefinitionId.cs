using NBB.Domain;

namespace Tasks.Runtime.Domain.ProcessDefinitionAggregate
{
    public record ProcessDefinitionId(int Value) : Identity<int>(Value)
    {
        public override string ToString() => Value.ToString();
    }
}
