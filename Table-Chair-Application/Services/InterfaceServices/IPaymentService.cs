using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Entity.Enums;
using Table_Chair_Application.Dtos.PaymentDtos;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(PaymentCreateDto paymentDto);
        Task<PaymentResponseDto> GetPaymentByIdAsync(int id);
        Task<IEnumerable<PaymentResponseDto>> GetPaymentsByOrderIdAsync(int orderId);
        Task<IEnumerable<PaymentResponseDto>> GetPaymentsByStatusAsync(PaymentStatus status);
        Task<IEnumerable<PaymentResponseDto>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<PaymentResponseDto> UpdatePaymentStatusAsync(int id, PaymentUpdateDto paymentDto);
        Task<PaymentResponseDto> ProcessPaymentCallbackAsync(PaymentCallbackDto callbackDto);
        Task<PaymentResponseDto> RefundPaymentAsync(int paymentId, RefundRequestDto refundDto);
        Task<PaginatedPaymentResponseDto> GetFilteredPaymentsAsync(PaymentFilterDto filter);
        Task<PaymentStatisticsDto> GetPaymentStatisticsAsync();
        Task<PaymentResponseDto> CancelPaymentAsync(int paymentId, string reason);
        Task<PaymentResponseDto> UpdatePaymentDetailsAsync(int id, PaymentDetailsUpdateDto updateDto);
    }
}
