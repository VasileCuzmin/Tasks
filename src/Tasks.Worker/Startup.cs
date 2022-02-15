using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections.Generic;
using Tasks.Definition.Domain.Repositories;
using Tasks.Domain.Data;

namespace Tasks.Worker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var topics = GetTopics();
            services.AddWorkerServices(Configuration, topics);

            //services.AddHealthChecks() // Registers health checks services
            //// Add a health check for a SQL database
            //   .AddCheck(new SqlConnectionHealthCheck("Tasks_Database", Configuration["ConnectionStrings:Tasks_Database"]));
            //services.AddSingleton<IHealthCheck, GCInfoHealthCheck>();
        }

        private IList<string> GetTopics()
        {
            var services = new ServiceCollection();
            services.AddSingleton(Configuration);
            services.AddLogging(x => x.AddSerilog());
            services.AddTasksDataAccess();

            using var container = services.BuildServiceProvider();
            var repo = container.GetRequiredService<IProcessDefinitionRepository>();
            var topics = repo.GetAllTopicsForEnabledProcesses().GetAwaiter().GetResult();
            return topics;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseHealthChecks("/healthz", options: new HealthCheckOptions()
            //{
            //    ResponseWriter = HealthCheckWriter.WriteResponse
            //});
        }
    }
}