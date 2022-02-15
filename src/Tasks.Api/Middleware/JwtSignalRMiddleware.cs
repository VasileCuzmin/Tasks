using Microsoft.AspNetCore.Builder;

namespace Tasks.Api.Middleware
{
    /// <summary>
    /// Middleware to intercept a query string bearer token value (since SignalR isn't
    /// able to use a Header) into an auth header so that the Jwt handler can see it.
    /// </summary>
    public static class JwtSignalRMiddleware
    {
        private static readonly string AUTH_QUERY_STRING_KEY = "access_token";
        public static void UseUrlAccessTokenAuthentication(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
                {
                    try
                    {
                        if (context.Request.QueryString.HasValue
                            && context.Request.Query.TryGetValue(AUTH_QUERY_STRING_KEY, out var token)
                            && !string.IsNullOrWhiteSpace(token))
                        {
                            context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                        }
                    }
                    catch
                    {
                        // if multiple headers it may throw an error.  Ignore both.
                    }
                }

                await next.Invoke();
            });
        }
    }
}