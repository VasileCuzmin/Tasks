using System;
using System.Collections.Immutable;
using System.Dynamic;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate;

namespace Tasks.RuntimeDomain.Tests.TaskAllocationAggregate.TaskAllocation_Test
{
    internal static class Setup
    {
        public static EventDefinitionName start = new("GenericStartEvent");
        public static EventDefinitionName bad = new(string.Empty);
        public static EventDefinitionName good = new("GenericEvent");
        public static EventDefinitionName cancel = new("GenericCancelEvent");
        public static EventDefinitionName stop = new("GenericStopEvent");
        public static DynamicEvent badEvent = new (bad, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
        public static DynamicEvent goodEvent = new (good, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
        public static DynamicEvent startEvent = new (start, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
        public static DynamicEvent cancelEvent = new (cancel, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);
        public static DynamicEvent stopEvent = new (stop, ImmutableDictionary.Create<string, object>(), Guid.NewGuid(), string.Empty);

        public static TaskAllocation GenerateTaskAllocationAggregate(TaskDefinition taskDefinition, TaskId taskId, ProcessId processId)
        {
            var taskAllocation = new TaskAllocation();
            taskAllocation.InitiateTask(taskId, processId, taskDefinition);
            return taskAllocation;
        }
    }
}
