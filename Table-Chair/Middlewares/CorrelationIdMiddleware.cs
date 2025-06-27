using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Table_Chair.Infrastructure.Logging.InterfaceCorrelationId;
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private const string CorrelationIdHeader = "X-Correlation-ID";
        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
        {
            try
            {
                var correlationId = GetCorrelationIdTrace(context, correlationIdGenerator);
                SetCorrelationIdToResponce(context, correlationId);
                await _next(context);
                _logger.LogInformation($"Correlation ID: {correlationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the request. Correlation ID: {correlationIdGenerator.Get()}");
                throw;
            }
            finally
            {
                correlationIdGenerator.Set(string.Empty);
            }

        }
        private static string GetCorrelationIdTrace(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
        {
            string? correlationId = context.Request.Headers.TryGetValue("X-Correlation-ID", out var values)
                ? values.FirstOrDefault()
                : null;

            string finalCorrelationId = string.IsNullOrWhiteSpace(correlationId)
                ? Guid.NewGuid().ToString()
                : correlationId;

            correlationIdGenerator.Set(finalCorrelationId);
            return finalCorrelationId;
        }


        private static void SetCorrelationIdToResponce(HttpContext context, string correlationId)
        {
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(CorrelationIdHeader))
                    context.Response.Headers.Append(CorrelationIdHeader, correlationId);

                return Task.CompletedTask;
            });
        }
    }
