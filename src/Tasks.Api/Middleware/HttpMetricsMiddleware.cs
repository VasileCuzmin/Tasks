using Microsoft.AspNetCore.Builder;

namespace Tasks.Api.Middleware
{
    public static class HttpMetricsMiddleware
    {
        public static void UseHttpTrafficMetric(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
           {
               var path = context.Request.Path.Value;
               var method = context.Request.Method;
               await next.Invoke();
               var statusCode = context.Response.StatusCode;
               HttpMetricsRegistry.MeasureHttpTrafficMetric(path, method, statusCode);
           });
        }
        public static void UseHttpLatencyMetric(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value;

                await HttpMetricsRegistry.MeasureHttpLatencyMetric(path, next);
            });
        }
    }
}