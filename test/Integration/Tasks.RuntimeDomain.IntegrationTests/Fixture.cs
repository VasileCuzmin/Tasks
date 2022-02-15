using Microsoft.Extensions.DependencyInjection;
using NBB.Messaging.Abstractions;
using NBB.Tools.ExpressionEvaluation.Abstractions;
using NBB.Tools.ExpressionEvaluation.DynamicExpresso;
using System;
using Tasks.Runtime.Domain;

namespace Tasks.RuntimeDomain.IntegrationTests
{
    public class Fixture
    {
        internal readonly IServiceProvider Container;

        public Fixture()
        {
            Container = BuildServiceProvider();
        }


        private IServiceProvider BuildServiceProvider()
        {
            //var configurationBuilder = new ConfigurationBuilder()
            //var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();
            //services.AddSingleton<IConfiguration>(configuration);
            //services.AddLogging();

            services.AddTasksRuntimeServices();
            services.AddSingleton<IExpressionEvaluator, ExpressionEvaluator>();
            services.AddSingleton<IMessageSerDes, NewtonsoftJsonMessageSerDes>();
            services.AddSingleton<IMessageTypeRegistry, DefaultMessageTypeRegistry>();
            
            // TODO: check if necessary
            //services.AddSingleton(sp =>
            //{
            //    var builder = new MessageSerDesOptionsBuilder();
            //    return builder.Options;
            //});

            var container = services.BuildServiceProvider();            
            return container;
        }
    }
}
