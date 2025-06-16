using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Payments
{
    public interface IPaymentGateway
    {
        Task<PaymentResult> ProcessPaymentAsync(Payment payment);
        Task<PaymentResult> RefundPaymentAsync(int paymentId, decimal amount);
        Task<PaymentResult> CheckPaymentStatusAsync(string transactionId);
        Task<PaymentResult> CapturePaymentAsync(string transactionId, decimal amount);
        Task<PaymentResult> VoidPaymentAsync(string transactionId);
    }
}
