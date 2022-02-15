using System;
using System.Collections.Immutable;
using Tasks.Runtime.Domain.EventDefinitionAggregate;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate
{
    public record JsonEvent(EventDefinitionName EventDefinitionName, string Json, Guid MessageId, string ApplicationName);
    public record DynamicEvent(EventDefinitionName EventDefinitionName, ImmutableDictionary<string, object> Payload, Guid MessageId, string ApplicationName);
}
