using Microsoft.Extensions.Options;

namespace Table_Chair.Middlewares
{
    public class GlobalLoggingMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        public GlobalLoggingMiddleware(ILogger<GlobalLoggingMiddleware> logger
            ,RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
