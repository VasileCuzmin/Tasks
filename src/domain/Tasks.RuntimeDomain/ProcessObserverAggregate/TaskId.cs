using System;
using NBB.Domain;
using Newtonsoft.Json;

namespace Tasks.Runtime.Domain.ProcessObserverAggregate
{
    public record TaskId : Identity<Guid>
    {
        [JsonConstructor]
        public TaskId(Guid value)
            : base(value)
        {
        }

        public TaskId()
            : this(Guid.NewGuid())
        {
        }

        public override string ToString() => Value.ToString();
    }
}