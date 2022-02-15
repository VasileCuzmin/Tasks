using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBB.Correlation.AspNet;
using NBB.Messaging.Abstractions;
using NBB.Messaging.OpenTracing.Publisher;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTracing;
using OpenTracing.Noop;
using OpenTracing.Util;
using Prometheus;
using System;
using System.Reflection;
using Tasks.Api.Decorators;
using Tasks.Api.Middleware;
using Tasks.Api.Services;
using Tasks.Api.Swagger;
using Tasks.Definition.Application;
using Tasks.Definition.Application.Commands;
using Tasks.Definition.Application.IntegrationEventHandlers;
using Tasks.Domain.Data;
using Tasks.PublishedLanguage.Commands;
using Tasks.Runtime.Application.Commands;

namespace Tasks.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddScopedContravariant<IRequestHandler<IRequest, Unit>, MessageBusPublisherCommandHandler>(typeof(Shutdown).Assembly);
            services.AddScopedContravariant<IRequestHandler<IRequest, Unit>, MessageBusPublisherCommandHandler>(typeof(CreateApplication).Assembly);
            services.AddScopedContravariant<IRequestHandler<IRequest, Unit>, MessageBusPublisherCommandHandler>(typeof(StartTask).Assembly);

            services.AddMediatR(new[] { typeof(StartTask).Assembly, typeof(CreateApplication).Assembly });
            services.AddScoped<IEventHub, EventHub>();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["Identity:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = Configuration["Identity:ApiName"];
                });

            services.AddTasksDataAccess();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUserService, UserService>();

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.Scan(scan => scan.FromAssemblyOf<LanguagePublishedEventHandler>()
              .AddClasses(classes => classes.AssignableTo<IValidator>())
              .AsImplementedInterfaces()
              .WithScopedLifetime());

            services.AddSwagger();

            var transport = Configuration.GetValue("Messaging:Transport", "NATS");
            if (transport.Equals("NATS", StringComparison.InvariantCultureIgnoreCase))
            {
                services.AddMessageBus().AddNatsTransport(Configuration).UseTopicResolutionBackwardCompatibility(Configuration);
            }
            else if (transport.Equals("Rusi", StringComparison.InvariantCultureIgnoreCase))
            {

                services.AddMessageBus().AddRusiTransport(Configuration).UseTopicResolutionBackwardCompatibility(Configuration);
            }
            else
            {
                throw new Exception($"Messaging:Transport={transport} not supported");
            }

            services.Decorate<IMessageBusPublisher, OpenTracingPublisherDecorator>();
            services.Decorate<IMessageBusPublisher, MessageBusPublisherDecorator>();

            services.AddProblemDetails(ConfigureProblemDetails);

            services.AddOpenTracing(openTracingBuilder =>
                openTracingBuilder.ConfigureGenericDiagnostics(options =>
                    options.IgnoredListenerNames.Add("SqlClientDiagnosticListener")));

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                if (!Configuration.GetValue<bool>("OpenTracing:Jeager:IsEnabled"))
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
                            Configuration.GetValue<string>("OpenTracing:Jeager:AgentHost"),
                            Configuration.GetValue<int>("OpenTracing:Jeager:AgentPort"), 0))
                        .Build())
                    .Build();

                GlobalTracer.Register(tracer);

                return tracer;
            });

            //services.Configure<AspNetCoreInstrumentationOptions>(this.Configuration.GetSection("AspNetCoreInstrumentation"));

            // For options which can be configured from code only.
            //services.Configure<AspNetCoreInstrumentationOptions>(options =>
            //{
            //    options.Filter = (req) =>
            //    {
            //        return req.Request.Host != null;
            //    };
            //});

            //services.AddOpenTelemetryMetrics(builder =>
            //{
            //    builder.AddAspNetCoreInstrumentation();

            //    builder.AddPrometheusExporter(opt =>
            //     {
            //         //opt.StartHttpListener = true;
            //         //opt.HttpListenerPrefixes = new string[] { $"http://localhost:{555554}/" };
            //     }).AddMeter("Meter");
            //});
        }

        private void ConfigureProblemDetails(ProblemDetailsOptions options)
        {
            options.IncludeExceptionDetails = (_context, _exception) => true;
            options.MapStatusCode = context => new StatusCodeProblemDetails(context.Response.StatusCode);

            // This will map NotImplementedException to the 501 Not Implemented status code.
            options.Map<NotImplementedException>(_ex => new StatusCodeProblemDetails(StatusCodes.Status501NotImplemented));

            options.Map<ValidationException>(_ex =>
                new StatusCodeProblemDetails(StatusCodes.Status422UnprocessableEntity));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseHttpTrafficMetric();

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseRouting();

            app.UseCors(cors =>
            {
                cors
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
            app.UseHttpMetrics();
            app.UseProblemDetails();
            app.UseCorrelation();
            app.UseUrlAccessTokenAuthentication();
            app.UseAuthentication();

            //app.UseHttpLatencyMetric();
          
            app.UseEndpoints(builder =>
            {
                builder.MapControllers();
                builder.MapMetrics();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tasks Api");
            });

            //   app.UseOpenTelemetryPrometheusScrapingEndpoint();
          
        }
    }
}