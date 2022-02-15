using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NBB.Domain;
using NBB.Messaging.Abstractions;
using NBB.Messaging.Host;
using NBB.Messaging.OpenTracing.Publisher;
using NBB.Messaging.OpenTracing.Subscriber;
using NBB.Tools.ExpressionEvaluation.Abstractions;
using NBB.Tools.ExpressionEvaluation.DynamicExpresso;
using OpenTracing;
using OpenTracing.Noop;
using OpenTracing.Util;
using System.Collections.Generic;
using System.Reflection;
using NBB.Application.DataContracts.Schema;
using Tasks.Definition.Application;
using Tasks.Definition.Application.Commands;
using Tasks.Definition.Application.Events;
using Tasks.Domain.Data;
using Tasks.PublishedLanguage.Commands;
using Tasks.PublishedLanguage.Events.Definition;
using Tasks.Runtime.Application.Commands;
using Tasks.Runtime.Domain;
using Tasks.Worker.MessageMiddlewares;
using Tasks.Worker.MessagingPipeline;
using NBB.Core.Pipeline;
using System;

namespace Tasks.Worker
{

    public static class DependencyInjectionExtensions
    {
        public static void AddWorkerServices(this IServiceCollection services, IConfiguration configuration, IEnumerable<string> topics, bool useTestDoubles = false)
        {
            services.AddTasksDataAccess(useTestDoubles);
            services.AddTasksRuntimeServices();

            services.AddSingleton<IExpressionEvaluator, ExpressionEvaluator>();

            //Event store
            if (useTestDoubles)
            {
                services.AddEventStore()
                    .WithNewtownsoftJsonEventStoreSeserializer(new[] { new SingleValueObjectConverter() })
                    .WithInMemoryEventRepository();
            }
            else
            {
                services.AddEventStore()
                    .WithNewtownsoftJsonEventStoreSeserializer(new[] { new SingleValueObjectConverter() })
                    .WithAdoNetEventRepository();
            }

            // MediatR & Messaging
            services.AddMediatR(typeof(ReceiveEvent.Command).Assembly, typeof(UpdateProcessDefinition.Command).Assembly);
            services.AddScopedContravariant<INotificationHandler<INotification>, MessageBusPublisherEventHandler>(typeof(ProcessUpdated).Assembly);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));


            if (useTestDoubles)
            {
                services.AddMessageBus().AddInProcessTransport();
            }
            else
            {
                var transport = configuration.GetValue("Messaging:Transport", "NATS");
                if (transport.Equals("NATS", StringComparison.InvariantCultureIgnoreCase))
                {
                    services.AddMessageBus().AddNatsTransport(configuration).UseTopicResolutionBackwardCompatibility(configuration);
                }
                else if (transport.Equals("Rusi", StringComparison.InvariantCultureIgnoreCase))
                {

                    services.AddMessageBus().AddRusiTransport(configuration).UseTopicResolutionBackwardCompatibility(configuration);
                }
                else
                {
                    throw new Exception($"Messaging:Transport={transport} not supported");
                }
            }

            services
                .Decorate<IMessageBusPublisher, OpenTracingPublisherDecorator>()
                .Decorate<IMessageBusPublisher, MessageBusPublisherDecorator>();

            services.AddMessagingHost(
                configuration,
                hostBuilder => hostBuilder
                .Configure(configBuilder =>
                {
                    static IPipelineBuilder<MessagingContext> basePipeline(IPipelineBuilder<MessagingContext> builder) => builder
                        .UseCorrelationMiddleware()
                        .UseExceptionHandlingMiddleware()
                        .UseMiddleware<OpenTracingMiddleware>()
                        .UseMiddleware<HandleExecutionErrorMiddleware>()
                        .UseDefaultResiliencyMiddleware();

                    configBuilder
                       .AddSubscriberServices(config => config.FromTopics(topics))
                       .WithDefaultOptions()
                       .UsePipeline(builder => basePipeline(builder)
                           .UseMiddleware<ReceiveEventMediatRMiddleware>());

                    configBuilder
                        .AddSubscriberServices(config => config
                            .FromMediatRHandledCommands().AddClassesWhere(x => x != typeof(Shutdown))
                            .FromMediatRHandledEvents().AddClassesWhere(x => x.Assembly == typeof(LanguagePublished).Assembly))
                        .WithDefaultOptions()
                        .UsePipeline(builder => basePipeline(builder)
                            .UseMediatRMiddleware());

                    configBuilder
                        .AddSubscriberServices(config => config.AddType<Shutdown>())
                            .WithOptions(config => config.ConfigureTransport(transportOpts => transportOpts with { UseGroup = false }))
                        .UsePipeline(builder => basePipeline(builder)
                            .UseMediatRMiddleware());

                    configBuilder
                        .AddSubscriberServices(config => config.AddType<SchemaDefinitionUpdated>())
                            .WithOptions(config => config.ConfigureTransport(t => t with { AckWait = 200000 }))
                        .UsePipeline(builder => basePipeline(builder)
                            .UseMediatRMiddleware());
                }));

            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddOpenTracingCoreServices(builder => builder
                .AddEntityFrameworkCore(o => o.LogEvents = false)
                .AddGenericDiagnostics()
                .AddLoggerProvider()
                .ConfigureGenericDiagnostics(options => options.IgnoredListenerNames.Add("SqlClientDiagnosticListener")));


            services.AddSingleton(serviceProvider =>
            {
                if (!configuration.GetValue<bool>("OpenTracing:Jeager:IsEnabled"))
                {
                    return NoopTracerFactory.Create();
                }

                string serviceName = Assembly.GetEntryAssembly().GetName().Name;

                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                ITracer tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(new ConstSampler(true))
                    .WithReporter(new RemoteReporter.Builder()
                        .WithSender(new UdpSender(
                            configuration.GetValue<string>("OpenTracing:Jeager:AgentHost"),
                            configuration.GetValue<int>("OpenTracing:Jeager:AgentPort"), 0))
                        .Build())
                    .Build();

                GlobalTracer.Register(tracer);

                return tracer;
            });
        }
    }
}