using System;
using System.Collections.Generic;
using System.Linq;
using NBB.Domain;

namespace Tasks.Runtime.Domain.EventDefinitionAggregate
{
    public record ProcessIdentifierProps
    {
        public IEnumerable<string> Props { get; init; }

        public ProcessIdentifierProps(IEnumerable<string> props)
            : this(props.ToArray())
        {
        }

        public ProcessIdentifierProps(params string[] props)
        {
            Props = props;
        }

        #nullable enable
        public virtual bool Equals(ProcessIdentifierProps? other) =>
            other != null ? Props.Intersect(other.Props).Count() == Props.Count() : Props == null;
        #nullable restore

        public override int GetHashCode()
        {
            var hash = new HashCode();

            foreach (var item in Props)
            {
                hash.Add(item);
            }

            return hash.ToHashCode();
        }
    }
}
