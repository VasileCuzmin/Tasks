using System;
using System.Linq;
using NBB.Tools.ExpressionEvaluation.Abstractions;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.Runtime.Domain.Services
{
    public class ExpressionEvaluationService : IExpressionEvaluationService
    {
        private readonly IExpressionEvaluator _expressionEvaluator;

        public ExpressionEvaluationService(IExpressionEvaluator expressionEvaluator)
        {
            _expressionEvaluator = expressionEvaluator;
        }

        public bool EvaluateEventExpression(DynamicEvent @event, DynamicExpression expression)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (expression?.Value == null)
            {
                return true;
            }

            var parameters = @event.Payload.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
            var result = _expressionEvaluator.Evaluate<bool>(expression.Value, parameters);

            return result;
        }
    }

}
