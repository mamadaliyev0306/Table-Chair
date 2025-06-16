using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Payments
{
    public class StripeGateway : PaymentGatewayService
    {
        private readonly string _apiKey;

        public StripeGateway(
            IConfiguration configuration,
            ILogger<StripeGateway> logger,
            HttpClient httpClient) : base(configuration, logger, httpClient)
        {
            // Fix for CS8601: Ensure the configuration value is not null
            _apiKey = _configuration["PaymentGateways:Stripe:ApiKey"]
                      ?? throw new ArgumentNullException(nameof(_configuration), "Stripe API key is not configured.");
        }

        public override async Task<PaymentResult> ProcessPaymentAsync(Payment payment)
        {
            StripeConfiguration.ApiKey = _apiKey;

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(payment.Amount * 100),
                Currency = payment.Currency.ToString(),
                PaymentMethodTypes = new List<string> { "card" },
                Description = $"Order #{payment.OrderId}",
                Metadata = new Dictionary<string, string>
                    {
                        { "order_id", payment.OrderId.ToString() },
                        { "payment_id", payment.Id.ToString() }
                    }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return new PaymentResult
            {
                Success = intent.Status == "requires_confirmation" || intent.Status == "requires_capture",
                TransactionId = intent.Id,
                GatewayResponse = intent.ToJson()
            };
        }

        public override async Task<PaymentResult> RefundPaymentAsync(int paymentId, decimal amount)
        {
            StripeConfiguration.ApiKey = _apiKey;

            var intentService = new PaymentIntentService();
            var paymentIntent = await intentService.GetAsync("pi_123"); // real ID DB dan olinadi

            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntent.Id,
                Amount = (long)(amount * 100)
            };

            var refundService = new RefundService();
            var refund = await refundService.CreateAsync(refundOptions);

            return new PaymentResult
            {
                Success = refund.Status == "succeeded",
                TransactionId = refund.Id,
                GatewayResponse = refund.ToJson()
            };
        }

        public override async Task<PaymentResult> CheckPaymentStatusAsync(string transactionId)
        {
            StripeConfiguration.ApiKey = _apiKey;

            var service = new PaymentIntentService();
            var intent = await service.GetAsync(transactionId);

            return new PaymentResult
            {
                Success = intent.Status == "succeeded",
                TransactionId = intent.Id,
                GatewayResponse = intent.ToJson()
            };
        }

        public override async Task<PaymentResult> CapturePaymentAsync(string transactionId, decimal amount)
        {
            StripeConfiguration.ApiKey = _apiKey;

            var service = new PaymentIntentService();
            var intent = await service.CaptureAsync(transactionId);

            return new PaymentResult
            {
                Success = intent.Status == "succeeded",
                TransactionId = intent.Id,
                GatewayResponse = intent.ToJson()
            };
        }

        public override async Task<PaymentResult> VoidPaymentAsync(string transactionId)
        {
            StripeConfiguration.ApiKey = _apiKey;

            var service = new PaymentIntentService();
            var canceled = await service.CancelAsync(transactionId);

            return new PaymentResult
            {
                Success = canceled.Status == "canceled",
                TransactionId = canceled.Id,
                GatewayResponse = canceled.ToJson()
            };
        }
    }
}

