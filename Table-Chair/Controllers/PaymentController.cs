using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos.PaymentDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Yangi to'lov yaratish", Description = "To'lov yaratish uchun ishlatiladi")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto paymentDto)
        {
            var result = await _paymentService.CreatePaymentAsync(paymentDto);
            return CreatedAtAction(nameof(GetPaymentById),
                new { id = result.Id },
                ApiResponse<PaymentResponseDto>.SuccessResponse(result, "To'lov yaratildi"));
        }

        [HttpGet("getbyId/{id}")]
        [SwaggerOperation(Summary = "ID bo'yicha to'lovni olish")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<PaymentResponseDto>.FailResponse("To'lov topilmadi"));
            return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(result));
        }

        [HttpGet("get-order-byId/{orderId}")]
        [SwaggerOperation(Summary = "Buyurtma ID bo'yicha to'lovlarni olish")]
        public async Task<IActionResult> GetPaymentsByOrderId(int orderId)
        {
            var result = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
            return Ok(ApiResponse<IEnumerable<PaymentResponseDto>>.SuccessResponse(result));
        }

        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerOperation(Summary = "To'lov statistikasi")]
        public async Task<IActionResult> GetPaymentStatistics()
        {
            var result = await _paymentService.GetPaymentStatisticsAsync();
            return Ok(ApiResponse<PaymentStatisticsDto>.SuccessResponse(result));
        }

        [HttpGet("filtered")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerOperation(Summary = "Filterlangan to'lovlar")]
        public async Task<IActionResult> GetFilteredPayments([FromQuery] PaymentFilterDto filter)
        {
            var result = await _paymentService.GetFilteredPaymentsAsync(filter);
            return Ok(ApiResponse<PaginatedPaymentResponseDto>.SuccessResponse(result));
        }

        [HttpPut("update-status/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerOperation(Summary = "To'lov holatini yangilash")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] PaymentUpdateDto paymentDto)
        {
            var result = await _paymentService.UpdatePaymentStatusAsync(id, paymentDto);
            return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(result, "To‘lov holati yangilandi"));
        }

        [HttpPost("refund/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerOperation(Summary = "To'lovni qaytarish (refund)")]
        public async Task<IActionResult> RefundPayment(int id, [FromBody] RefundRequestDto refundDto)
        {
            var result = await _paymentService.RefundPaymentAsync(id, refundDto);
            return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(result, "To‘lov qaytarildi"));
        }

        [HttpPost("cancel/{id}")]
        [SwaggerOperation(Summary = "To'lovni bekor qilish")]
        public async Task<IActionResult> CancelPayment(int id, [FromBody] CancelPaymentRequestDto request)
        {
            var result = await _paymentService.CancelPaymentAsync(id, request.Reason);
            return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(result, "To‘lov bekor qilindi"));
        }

        [HttpPut("update-details/{id}")]
        [SwaggerOperation(Summary = "To'lov ma'lumotlarini yangilash")]
        public async Task<IActionResult> UpdatePaymentDetails(int id, [FromBody] PaymentDetailsUpdateDto updateDto)
        {
            var result = await _paymentService.UpdatePaymentDetailsAsync(id, updateDto);
            return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(result, "To‘lov ma’lumotlari yangilandi"));
        }

        [HttpPost("callback")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Payment provider callback")]
        public async Task<IActionResult> ProcessPaymentCallback([FromBody] PaymentCallbackDto callbackDto)
        {
            var result = await _paymentService.ProcessPaymentCallbackAsync(callbackDto);
            return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(result, "Callback muvaffaqiyatli bajarildi"));
        }
    }
}
