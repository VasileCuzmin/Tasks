using Tasks.Runtime.Domain.EventDefinitionAggregate;

namespace Tasks.Runtime.Domain.ProcessDefinitionAggregate
{
    public record TaskDefinition(
        string Name, 
        EventDefinitionName StartEvent, 
        EventDefinitionName CloseEvent, 
        EventDefinitionName CancelEvent,
        DynamicExpression StartExpression, 
        DynamicExpression CloseExpression,
        DynamicExpression CancelExpression, 
        DynamicExpression UserAllocationExpression, 
        DynamicExpression GroupAllocationExpression, 
        AutomaticStart AutomaticStart
    );    
}