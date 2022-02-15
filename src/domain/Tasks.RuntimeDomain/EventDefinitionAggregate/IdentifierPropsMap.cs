using System;
using System.Collections.Generic;
using System.Linq;

namespace Tasks.Runtime.Domain.EventDefinitionAggregate
{
    public record IdentifierPropsMap
    {
        public Dictionary<string, string> Map { get; init; }

        private static string MapEventIdentifiersWithProcessIdentifiers(List<string> processProps, List<string> eventProps, int processIdentifierPropIndex)
         => eventProps.ElementAtOrDefault(processIdentifierPropIndex) ?? processProps[processIdentifierPropIndex];

        public static IdentifierPropsMap From(string processProps, string eventProps)
        {
            if (eventProps == null)
                throw new ArgumentNullException(nameof(eventProps));
            if (processProps == null)
                throw new ArgumentNullException(nameof(processProps));

            return MapEventIdentifierToProcessIdentifiers(processProps, eventProps);
        }


        private IdentifierPropsMap(Dictionary<string, string> map)
            => Map = map;

        private static IdentifierPropsMap MapEventIdentifierToProcessIdentifiers(string eventProps, string processProps)
        {
            var map = new Dictionary<string, string>();

            foreach (var item in processProps.Split(';').ToList())
            {
                var mappedValue = MapEventIdentifiersWithProcessIdentifiers(processProps.Split(';').ToList(), eventProps.Split(';').ToList(), processProps.Split(';').ToList().IndexOf(item));
                map.Add(mappedValue, string.IsNullOrEmpty(item) ? mappedValue : item);
            }

            return new IdentifierPropsMap(map);
        }

        #nullable enable
        public virtual bool Equals(IdentifierPropsMap? other) =>
           other != null ? Map.Intersect(other.Map).Count() == Map.Count : Map == null;
        #nullable restore

        public override int GetHashCode()
        {
            var hash = new HashCode();

            foreach (var item in Map)
            {
                hash.Add(item);
            }

            return hash.ToHashCode();
        }
    }
}