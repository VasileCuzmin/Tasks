using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NBB.Core.Abstractions;
using NBB.Data.EntityFramework;
using NBB.Data.EventSourcing;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;
using Tasks.Runtime.Domain.ProcessObserverAggregate;
using Tasks.Runtime.Domain.TaskAllocationAggregate;
using IEventDefinitionRepository = Tasks.Runtime.Domain.EventDefinitionAggregate.IEventDefinitionRepository;
using IProcessDefinitionRepository = Tasks.Runtime.Domain.ProcessDefinitionAggregate.IProcessDefinitionRepository;

namespace Tasks.Domain.Data
{
    public static class DependencyInjectionExtensions
    {
        public static void AddTasksDataAccess(this IServiceCollection services, bool useTestDoubles = false)
        {
            services.AddEntityFrameworkDataAccess();
            services.AddEventSourcingDataAccess((sp, builder) => 
                builder.Options.DefaultSnapshotVersionFrequency = 10);

            if (useTestDoubles)
            {
                services
                    .AddDbContextPool<TasksDefinitionDbContext>(options => options.UseInMemoryDatabase("TasksTests"));
            }
            else
            {
                services
                    .AddDbContextPool<TasksDefinitionDbContext>(
                        (serviceProvider, options) =>
                        {
                            var configuration = serviceProvider.GetService<IConfiguration>();
                            var connectionString = configuration.GetConnectionString("Tasks_Database");

                            options
                                .UseSqlServer(connectionString, builder => { builder.EnableRetryOnFailure(3); })
                                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                        });
            }

            // Definitions
            services.AddApplicationRepository();
            services.AddProcessDefinitionRepository();
            services.AddEventDefinitionRepository();
            services.AddProcessEventDefinitionRepository();
            services.AddTaskDefinitionRepository();

            // Runtime
            services.AddScoped<IEventDefinitionRepository, Repositories.Runtime.EventDefinitionRepository>();
            services.AddScoped<IProcessDefinitionRepository, Repositories.Runtime.ProcessDefinitionRepository>();
            services.AddEventSourcedRepository<ProcessObserver>();
            services.AddEventSourcedRepository<TaskAllocation>();
        }

        private static void AddApplicationRepository(this IServiceCollection services)
        {
            services.AddScoped<IUow<Application>, EfUow<Application, TasksDefinitionDbContext>>();
            services.AddScoped<IApplicationRepository, Repositories.Definition.ApplicationRepository>();
        }

        private static void AddEventDefinitionRepository(this IServiceCollection services)
        {
            services.AddScoped<IUow<EventDefinition>, EfUow<EventDefinition, TasksDefinitionDbContext>>();
            services.AddScoped<Definition.Domain.Repositories.IEventDefinitionRepository, Repositories.Definition.EventDefinitionRepository>();
        }

        private static void AddProcessDefinitionRepository(this IServiceCollection services)
        {
            services.AddScoped<IUow<ProcessDefinition>, EfUow<ProcessDefinition, TasksDefinitionDbContext>>();
            services.AddScoped<Definition.Domain.Repositories.IProcessDefinitionRepository, Repositories.Definition.ProcessDefinitionRepository>();
        }

        private static void AddProcessEventDefinitionRepository(this IServiceCollection services)
        {
            services.AddScoped<IUow<ProcessEventDefinition>, EfUow<ProcessEventDefinition, TasksDefinitionDbContext>>();
            services.AddScoped<IProcessEventDefinitionRepository, Repositories.Definition.ProcessEventDefinitionRepository>();
        }

        private static void AddTaskDefinitionRepository(this IServiceCollection services)
        {
            services.AddScoped<IUow<TaskDefinition>, EfUow<TaskDefinition, TasksDefinitionDbContext>>();
            services.AddScoped<ITaskDefinitionRepository, Repositories.Definition.TaskDefinitionRepository>();
        }
    }
}
