using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos.PaymentDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Yangi to'lov yaratish", Description = "To'lov yaratish uchun ishlatiladi")]
        [ProducesResponseType(typeof(PaymentResponseDto), 201)]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto paymentDto)
        {
            var result = await _paymentService.CreatePaymentAsync(paymentDto);
            return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
        }

        [HttpGet("getbyId/{id}")]
        [SwaggerOperation(Summary = "ID bo'yicha to'lovni olish")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("get-order-byId/{orderId}")]
        [SwaggerOperation(Summary = "Buyurtma ID bo'yicha to'lovlarni olish")]
        [ProducesResponseType(typeof(IEnumerable<PaymentResponseDto>), 200)]
        public async Task<IActionResult> GetPaymentsByOrderId(int orderId)
        {
            var result = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
            return Ok(result);
        }

        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerOperation(Summary = "To'lov statistikasi")]
        [ProducesResponseType(typeof(PaymentStatisticsDto), 200)]
        public async Task<IActionResult> GetPaymentStatistics()
        {
            var result = await _paymentService.GetPaymentStatisticsAsync();
            return Ok(result);
        }

        [HttpGet("filtered")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerOperation(Summary = "Filterlangan to'lovlar")]
        [ProducesResponseType(typeof(PaginatedPaymentResponseDto), 200)]
        public async Task<IActionResult> GetFilteredPayments([FromQuery] PaymentFilterDto filter)
        {
            var result = await _paymentService.GetFilteredPaymentsAsync(filter);
            return Ok(result);
        }

        [HttpPut("update-status/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerOperation(Summary = "To'lov holatini yangilash")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] PaymentUpdateDto paymentDto)
        {
            var result = await _paymentService.UpdatePaymentStatusAsync(id, paymentDto);
            return Ok(result);
        }

        [HttpPost("refund/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerOperation(Summary = "To'lovni qaytarish (refund)")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        public async Task<IActionResult> RefundPayment(int id, [FromBody] RefundRequestDto refundDto)
        {
            var result = await _paymentService.RefundPaymentAsync(id, refundDto);
            return Ok(result);
        }

        [HttpPost("cancel/{id}")]
        [SwaggerOperation(Summary = "To'lovni bekor qilish")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        public async Task<IActionResult> CancelPayment(int id, [FromBody] CancelPaymentRequestDto request)
        {
            var result = await _paymentService.CancelPaymentAsync(id, request.Reason);
            return Ok(result);
        }

        [HttpPut("update-details/{id}")]
        [SwaggerOperation(Summary = "To'lov ma'lumotlarini yangilash")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        public async Task<IActionResult> UpdatePaymentDetails(int id, [FromBody] PaymentDetailsUpdateDto updateDto)
        {
            var result = await _paymentService.UpdatePaymentDetailsAsync(id, updateDto);
            return Ok(result);
        }

        [HttpPost("callback")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Payment provider callback")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ProcessPaymentCallback([FromBody] PaymentCallbackDto callbackDto)
        {
            var result = await _paymentService.ProcessPaymentCallbackAsync(callbackDto);
            return Ok(result);
        }
    }
}