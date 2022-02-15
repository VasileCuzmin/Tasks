using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tasks.Api
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScopedContravariant<TBase, TResolve>(this IServiceCollection serviceCollection, Assembly assembly = null)
        {
            if (!typeof(TBase).IsGenericType || typeof(TBase).GetTypeInfo().IsGenericTypeDefinition)
                return;

            var baseDescription = typeof(TBase).GetGenericTypeDefinition();
            var typeArguments = typeof(TBase).GetGenericArguments();
            var firstTypeArgument = typeArguments.First();
            var restTypeArguments = typeArguments.Skip(1);

            var types = (assembly ?? firstTypeArgument.Assembly).ScanFor(firstTypeArgument);
            foreach (var t in types)
                serviceCollection.AddScoped(baseDescription.MakeGenericType(restTypeArguments.Prepend(t).ToArray()), typeof(TResolve));
        }

        public static IEnumerable<Type> ScanFor(this Assembly assembly, Type assignableType)
        {
            return assembly.GetTypes().Where(t => assignableType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        }
    }
}