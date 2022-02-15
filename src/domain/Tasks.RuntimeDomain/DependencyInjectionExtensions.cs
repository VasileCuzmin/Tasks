using Microsoft.Extensions.DependencyInjection;
using Tasks.Runtime.Domain.Services;

namespace Tasks.Runtime.Domain
{
    public static class DependencyExtensionExtensions
    {
        public static void AddTasksRuntimeServices(this IServiceCollection services)
        {
            services.AddScoped<EventRouterService>();
            services.AddSingleton<IExpressionEvaluationService, ExpressionEvaluationService>();
        }
    }
}