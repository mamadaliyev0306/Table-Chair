using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Payments
{
    public abstract class PaymentGatewayService : IPaymentGateway
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger<PaymentGatewayService> _logger;
        protected readonly HttpClient _httpClient;

        public PaymentGatewayService(
            IConfiguration configuration,
            ILogger<PaymentGatewayService> logger,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public abstract Task<PaymentResult> ProcessPaymentAsync(Payment payment);
        public abstract Task<PaymentResult> RefundPaymentAsync(int paymentId, decimal amount);
        public abstract Task<PaymentResult> CheckPaymentStatusAsync(string transactionId);
        public abstract Task<PaymentResult> CapturePaymentAsync(string transactionId, decimal amount);
        public abstract Task<PaymentResult> VoidPaymentAsync(string transactionId);

        protected virtual Task<PaymentResult> HandleExceptionAsync(Exception ex, string operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            _logger.LogError(ex, $"Payment gateway error during {operation}");

            return Task.FromResult(new PaymentResult
            {
                Success = false,
                ErrorMessage = $"Payment processing failed: {ex.Message}",
                GatewayResponse = ex.ToString()
            });
        }
    }
}

