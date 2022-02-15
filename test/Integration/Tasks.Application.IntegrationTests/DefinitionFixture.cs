using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Tasks.Definition.Application;
using Tasks.Definition.Application.Commands;
using Tasks.Definition.Application.IntegrationEventHandlers;
using Tasks.Domain.Data;
using Tasks.Migrations;

namespace Tasks.Application.IntegrationTests
{
    public class DefinitionFixture : IDisposable
    {        
        protected internal readonly IServiceProvider Container;              

        public DefinitionFixture()
        {            
            Container = BuildServiceProvider();
            Task.Run(PrepareDbAsync).Wait();
        }

        private async Task PrepareDbAsync()
        {
            var configuration = Container.GetService<IConfiguration>();            
            if (!ShouldCleanDb(configuration))
                return;

            var logConnectionString = configuration.GetConnectionString("Logs");
            using (var cnx = new SqlConnection(logConnectionString))
            {
                cnx.Open();

                var cmd = new SqlCommand("TRUNCATE TABLE [dbo].[__Logs]", cnx);
                cmd.ExecuteNonQuery();
            }

            var migrator = new DatabaseMigrator("DefaultConnection");
            await migrator.MigrateToLatestVersion();
        }        

        public void Dispose()
        {            
            Log.CloseAndFlush();
        }

        private IServiceProvider BuildServiceProvider()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddLogging();

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.Scan(scan => scan.FromAssemblyOf<LanguagePublishedEventHandler>()
                .AddClasses(classes => classes.AssignableTo<IValidator>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddTasksDataAccess(true);
            services.AddMediatR(typeof(CreateProcessDefinition.Handler).Assembly);
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddMessageBus().AddInProcessTransport();

            var container = services.BuildServiceProvider();            
            return container;
        }

        protected internal bool ShouldCleanDb(IConfiguration configuration)
        {
            var result = configuration.GetSection("App").GetValue<bool>("CleanupDb");
            return result;
        }
    }
}