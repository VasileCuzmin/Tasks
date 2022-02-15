using Microsoft.Extensions.DependencyInjection;
using NBB.Core.Abstractions;
using NBB.Data.EntityFramework;
using Tasks.Definition.Domain.Entities;
using Tasks.Definition.Domain.Repositories;
using Tasks.Domain.Data;
using Tasks.Domain.Data.Repositories.Definition;

namespace Tasks.Application.IntegrationTests
{
    public static class DependencyInjectionExtensions
    {
        public static void AddApplicationRepository(this IServiceCollection services)
        {            
            services.AddScoped<IUow<Definition.Domain.Entities.Application>, EfUow<Definition.Domain.Entities.Application, TasksDefinitionDbContext>>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
        }

        public static void AddEventDefinitionRepository(this IServiceCollection services)
        {
            services.AddScoped<IUow<EventDefinition>, EfUow<EventDefinition, TasksDefinitionDbContext>>();
            services.AddScoped<IEventDefinitionRepository, EventDefinitionRepository>();
        }

        public static void AddProcessDefinitionRepository(this IServiceCollection services)
        {
            services.AddScoped<IUow<ProcessDefinition>, EfUow<ProcessDefinition, TasksDefinitionDbContext>>();
            services.AddScoped<IProcessDefinitionRepository, ProcessDefinitionRepository>();
        }

        public static void AddProcessEventDefinitionRepository(this IServiceCollection services)
        {
            services.AddScoped<IUow<ProcessEventDefinition>, EfUow<ProcessEventDefinition, TasksDefinitionDbContext>>();
            services.AddScoped<IProcessEventDefinitionRepository, ProcessEventDefinitionRepository>();
        }

        public static void AddTaskDefinitionRepository(this IServiceCollection services)
        {
            services.AddScoped<IUow<TaskDefinition>, EfUow<TaskDefinition, TasksDefinitionDbContext>>();
            services.AddScoped<ITaskDefinitionRepository, TaskDefinitionRepository>();
        }
    }
}