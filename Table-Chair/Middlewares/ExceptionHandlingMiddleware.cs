using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Table_Chair_Application.Exceptions;
using NotFoundException = SendGrid.Helpers.Errors.Model.NotFoundException;

namespace Table_Chair.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Keyingi middleware yoki controllerga uzatadi
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                case Table_Chair_Application.Exceptions.BadRequestException:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "Internal server error.";
                    break;
            }

            var response = new
            {
                error = message,
                statusCode = (int)status
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}

