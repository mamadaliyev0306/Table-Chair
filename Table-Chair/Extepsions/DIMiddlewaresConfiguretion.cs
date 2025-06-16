using Table_Chair.Middlewares;

namespace Table_Chair.Extepsions
{
    public static class DIMiddlewaresConfiguretion
    {
        public static IApplicationBuilder UseMiddlewaresDI(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalLoggingMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            return app;
        }
    }
}
