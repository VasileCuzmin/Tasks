using System.Collections.Generic;
using Tasks.Runtime.Domain.EventDefinitionAggregate;
using Tasks.Runtime.Domain.ProcessDefinitionAggregate;

namespace Tasks.RuntimeDomain.Tests.Services.EventRouterService_Test
{
    internal static class Setup
    {
        public static HashSet<TaskDefinition> runtimeTaskDefinitions = new HashSet<TaskDefinition>()
        {
          new TaskDefinition("Elaborare Oferta", new EventDefinitionName("OfferCreated"),
               new EventDefinitionName("OfferStateUpdated"), null, null, null, null, new DynamicExpression("Administratori"), new DynamicExpression("Administratori"),new AutomaticStart(true))
        };

        public static Dictionary<ProcessDefinitionId, IdentifierPropsMap> dict = new Dictionary<ProcessDefinitionId, IdentifierPropsMap>()
        {
            {new ProcessDefinitionId(1),  IdentifierPropsMap.From("DocumentId;SiteId","DocumentId;SiteId") }
        };
    }
}