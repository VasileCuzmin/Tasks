using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.Task_Test
{
    internal static class Setup
    {
        public static Task CreateTaskWithExpressions()
        {
            var startEventDefinitionId = new EventDefinitionName("GenericStartEvent");
            var closeEventDefinitionId = new EventDefinitionName("GenericCloseEvent");
            var cancelEventDefinitionId = new EventDefinitionName("GenericCancelEvent");

            var startExpression = new DynamicExpression("GenericStartExpression");
            var closeExpression = new DynamicExpression("GenericCloseExpression");
            var cancelExpression = new DynamicExpression("GenericCancelExpression");
            var userExpression = new DynamicExpression("UserExpression");
            var groupExpression = new DynamicExpression("UserExpression");

            var taskDefinition = new TaskDefinition("GenericTask", startEventDefinitionId, closeEventDefinitionId,
                cancelEventDefinitionId, startExpression, closeExpression, cancelExpression, userExpression, groupExpression, new AutomaticStart(true));
            return new Task(new TaskId(), taskDefinition);
        }

        public static Task CreateTaskWithNullExpressions()
        {
            var startEventDefinitionId = new EventDefinitionName("GenericStartEvent");
            var closeEventDefinitionId = new EventDefinitionName("GenericCloseEvent");
            var cancelEventDefinitionId = new EventDefinitionName("GenericCancelEvent");

            var taskDefinition = new TaskDefinition("GenericTask", startEventDefinitionId, closeEventDefinitionId,
                cancelEventDefinitionId, null, null, null, null, null, new AutomaticStart(true));
            return new Task(new TaskId(), taskDefinition);
        }
    }
}
