using System.Collections.Generic;
using NBB.Domain;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate
{
    public record ImmutableKeyValue(string Key, string Value)
    {
        public override string ToString()
        {
            return $"{Key}:{Value}";
        }
    }
}