using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.Runtime.Domain.Services
{
    public interface IExpressionEvaluationService
    {
        bool EvaluateEventExpression(DynamicEvent @event, DynamicExpression expression);
    }
}