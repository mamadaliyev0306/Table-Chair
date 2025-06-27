using Table_Chair.Infrastructure.Logging;
using Table_Chair.Middlewares;

namespace Table_Chair.Extepsions
{
    public static class DIMiddlewaresConfiguretion
    {
        public static IApplicationBuilder UseMiddlewaresDI(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalLoggingMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();
           // app.UseMiddleware<RequestLoggingMiddleware>();
            return app;
        }
    }
}
