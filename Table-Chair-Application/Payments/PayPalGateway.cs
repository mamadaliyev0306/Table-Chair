using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Payments
{
    public class PayPalGateway : PaymentGatewayService
    {
        public PayPalGateway(
            IConfiguration configuration,
            ILogger<PayPalGateway> logger,
            HttpClient httpClient) : base(configuration, logger, httpClient)
        {
        }

        public override Task<PaymentResult> ProcessPaymentAsync(Payment payment)
        {
            return Task.FromResult(new PaymentResult
            {
                Success = true,
                TransactionId = "paypal_txn_001",
                GatewayResponse = "Mock PayPal ProcessPayment"
            });
        }

        public override Task<PaymentResult> RefundPaymentAsync(int paymentId, decimal amount)
        {
            return Task.FromResult(new PaymentResult
            {
                Success = true,
                TransactionId = "paypal_refund_001",
                GatewayResponse = "Mock PayPal Refund"
            });
        }

        public override Task<PaymentResult> CheckPaymentStatusAsync(string transactionId)
        {
            return Task.FromResult(new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                GatewayResponse = "Mock PayPal CheckStatus"
            });
        }

        public override Task<PaymentResult> CapturePaymentAsync(string transactionId, decimal amount)
        {
            return Task.FromResult(new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                GatewayResponse = "Mock PayPal Capture"
            });
        }

        public override Task<PaymentResult> VoidPaymentAsync(string transactionId)
        {
            return Task.FromResult(new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                GatewayResponse = "Mock PayPal Void"
            });
        }
    }
}


