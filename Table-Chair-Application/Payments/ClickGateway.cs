using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Payments
{
    public class ClickGateway : PaymentGatewayService
    {
        public ClickGateway(
            IConfiguration configuration,
            ILogger<ClickGateway> logger,
            HttpClient httpClient) : base(configuration, logger, httpClient)
        {
        }

        public override Task<PaymentResult> ProcessPaymentAsync(Payment payment)
        {
            return Task.FromResult(new PaymentResult
            {
                Success = true,
                TransactionId = "click_txn_001",
                GatewayResponse = "Mock Click ProcessPayment"
            });
        }

        public override Task<PaymentResult> RefundPaymentAsync(int paymentId, decimal amount)
        {
            return Task.FromResult(new PaymentResult
            {
                Success = true,
                TransactionId = "click_refund_001",
                GatewayResponse = "Mock Click Refund"
            });
        }

        public override Task<PaymentResult> CheckPaymentStatusAsync(string transactionId)
        {
            return Task.FromResult(new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                GatewayResponse = "Mock Click CheckStatus"
            });
        }

        public override Task<PaymentResult> CapturePaymentAsync(string transactionId, decimal amount)
        {
            return Task.FromResult(new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                GatewayResponse = "Mock Click Capture"
            });
        }

        public override Task<PaymentResult> VoidPaymentAsync(string transactionId)
        {
            return Task.FromResult(new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                GatewayResponse = "Mock Click Void"
            });
        }
    }
}


