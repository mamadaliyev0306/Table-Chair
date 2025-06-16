using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Payments
{
    public class PaymeGateway : PaymentGatewayService
    {
        private readonly string _merchantId;
        private readonly string _secretKey;

        public PaymeGateway(
            IConfiguration configuration,
            ILogger<PaymeGateway> logger,
            HttpClient httpClient) : base(configuration, logger, httpClient)
        {
            _merchantId = configuration["PaymentGateways:Payme:MerchantId"]
                          ?? throw new ArgumentNullException(nameof(configuration), "MerchantId is not configured.");
            _secretKey = configuration["PaymentGateways:Payme:SecretKey"]
                         ?? throw new ArgumentNullException(nameof(configuration), "SecretKey is not configured.");
        }

        public override async Task<PaymentResult> ProcessPaymentAsync(Payment payment)
        {
            try
            {
                var request = new
                {
                    method = "cards.create",
                    @params = new
                    {
                        card = new { number = "8600123412341234", expire = "03/30" },
                        amount = (int)(payment.Amount * 100), // tiyin
                        currency = payment.Currency ,
                        description = $"Order #{payment.OrderId}",
                        account = new { order_id = payment.OrderId }
                    }
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Auth", $"{_merchantId}:{_secretKey}");

                var response = await _httpClient.PostAsync("https://checkout.paycom.uz/api", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        ErrorMessage = "Payme payment failed",
                        GatewayResponse = responseString
                    };
                }

                var result = JsonSerializer.Deserialize<PaymeResponse>(responseString);

                return new PaymentResult
                {
                    Success = result?.Error == null,
                    TransactionId = result?.Result?.Card?.Token,
                    GatewayResponse = responseString,
                    ErrorMessage = result?.Error?.Message
                };
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(ex, "Payme payment processing");
            }
        }

        public override Task<PaymentResult> RefundPaymentAsync(int paymentId, decimal amount)
            => Task.FromResult(new PaymentResult { Success = false, ErrorMessage = "Not implemented" });

        public override Task<PaymentResult> CheckPaymentStatusAsync(string transactionId)
            => Task.FromResult(new PaymentResult { Success = false, ErrorMessage = "Not implemented" });

        public override Task<PaymentResult> CapturePaymentAsync(string transactionId, decimal amount)
            => Task.FromResult(new PaymentResult { Success = false, ErrorMessage = "Not implemented" });

        public override Task<PaymentResult> VoidPaymentAsync(string transactionId)
            => Task.FromResult(new PaymentResult { Success = false, ErrorMessage = "Not implemented" });

        private class PaymeResponse
        {
            public PaymeError? Error { get; set; }
            public PaymeResult? Result { get; set; }
        }

        private class PaymeError
        {
            public string? Message { get; set; }
        }

        private class PaymeResult
        {
            public PaymeCard? Card { get; set; }
        }

        private class PaymeCard
        {
            public string? Token { get; set; }
        }
    }
}

