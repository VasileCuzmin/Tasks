using NBB.Domain;

namespace Tasks.Runtime.Domain.ProcessDefinitionAggregate
{
    public record DynamicExpression(string StrExpression) : SingleValueObject<string>(StrExpression);
}
