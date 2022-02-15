using System.Collections.Generic;
using System.Linq;
using NBB.Domain;
using Newtonsoft.Json;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate
{
    public record ProcessId : SingleValueObject<IEnumerable<ImmutableKeyValue>>
    {
        [JsonConstructor]
        public ProcessId(IEnumerable<ImmutableKeyValue> keyValues)
            : base(keyValues.ToList())
        {
        }

        public ProcessId(params ImmutableKeyValue[] keyValues)
            : base(keyValues.ToList())
        {
        }

        public ProcessId(string serializedKeys)
            : base(ExtractKeyValues(serializedKeys))
        {
        }

        public override string ToString() => string.Join(";", Value);

        private static IEnumerable<ImmutableKeyValue> ExtractKeyValues(string serializedKeys)
        {
            var keys = serializedKeys.Split(';');
            var list = keys.Select(key => key.Split(':')).Select(values => new ImmutableKeyValue(values[0], values[1])).ToList();

            return list;
        }
    }
}
