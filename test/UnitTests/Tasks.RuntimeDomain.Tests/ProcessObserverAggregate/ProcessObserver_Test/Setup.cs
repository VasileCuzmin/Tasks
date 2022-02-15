using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessObserverAggregate;

namespace Tasks.RuntimeDomain.Tests.ProcessObserverAggregate.ProcessObserver_Test
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

        public static ProcessObserver GenerateProcessObserver(params TaskDefinition[] taskDefinitions)
        {
            var processDefinitionId = new ProcessDefinitionId(default);
            var keyValue = new ImmutableKeyValue(string.Empty, string.Empty);
            var processId = new ProcessId(keyValue);
            var processObserverId = new ProcessObserverId(processDefinitionId, processId);
            var hashTaskDefinitions = new HashSet<TaskDefinition>(taskDefinitions);
            return ProcessObserver.New(processObserverId, hashTaskDefinitions);
        }
    }
}