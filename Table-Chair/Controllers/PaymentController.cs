using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.PaymentDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Security;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Exceptions;

namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Barcha endpointlar auth talab qiladi
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IPaymentService paymentService,
            ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Yangi to'lov yaratish
        /// </summary>
        /// <param name="paymentDto">To'lov ma'lumotlari</param>
        /// <returns>Yaratilgan to'lov</returns>
        /// POST: https://localhost:7179/api/payment/create
        [HttpPost("create")]
        [ProducesResponseType(typeof(PaymentResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto paymentDto)
        {
            try
            {
                _logger.LogInformation("Yangi to'lov yaratish so'rovi qabul qilindi");
                var result = await _paymentService.CreatePaymentAsync(paymentDto);
                return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Noto'g'ri ma'lumotlar: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Buyurtma topilmadi: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "To'lov yaratishda xatolik");
                return StatusCode(500, "To'lov yaratishda ichki server xatosi");
            }
        }

        /// <summary>
        /// ID bo'yicha to'lovni olish
        /// </summary>
        /// <param name="id">To'lov IDsi</param>
        /// <returns>To'lov ma'lumotlari</returns>
        /// GET: https://localhost:7179/api/payment/getbyId/id
        [HttpGet("getbyId/{id}")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                _logger.LogInformation("To'lov ma'lumotlarini olish so'rovi. ID: {PaymentId}", id);
                var result = await _paymentService.GetPaymentByIdAsync(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("To'lov topilmadi: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "To'lov ma'lumotlarini olishda xatolik. ID: {PaymentId}", id);
                return StatusCode(500, "To'lov ma'lumotlarini olishda ichki server xatosi");
            }
        }

        /// <summary>
        /// Buyurtma IDsi bo'yicha to'lovlarni olish
        /// </summary>
        /// <param name="orderId">Buyurtma IDsi</param>
        /// <returns>To'lovlar ro'yxati</returns>
        /// GET: https://localhost:7179/api/payment/get-order-byId/id
        [HttpGet("get-order-byId/{orderId}")]
        [ProducesResponseType(typeof(IEnumerable<PaymentResponseDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetPaymentsByOrderId(int orderId)
        {
            try
            {
                _logger.LogInformation("Buyurtma to'lovlarini olish so'rovi. OrderID: {OrderId}", orderId);
                var result = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Buyurtma to'lovlarini olishda xatolik. OrderID: {OrderId}", orderId);
                return StatusCode(500, "Buyurtma to'lovlarini olishda ichki server xatosi");
            }
        }

        /// <summary>
        /// To'lov statistikasini olish
        /// </summary>
        /// <returns>To'lov statistikasi</returns>
        /// GET: https://localhost:7179/api/payment/statistics
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(PaymentStatisticsDto), 200)]
        [ProducesResponseType(401)]
        [Authorize(Roles = "Admin,Manager")] // Faqat admin va manager uchun
        public async Task<IActionResult> GetPaymentStatistics()
        {
            try
            {
                _logger.LogInformation("To'lov statistikasini olish so'rovi");
                var result = await _paymentService.GetPaymentStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "To'lov statistikasini olishda xatolik");
                return StatusCode(500, "Statistika olishda ichki server xatosi");
            }
        }

        /// <summary>
        /// Filterlangan to'lovlarni olish
        /// </summary>
        /// <param name="filter">Filter parametrlari</param>
        /// <returns>Pagelangan to'lovlar ro'yxati</returns>
        /// GET: https://localhost:7179/api/payment/filtered
        [HttpGet("filtered")]
        [ProducesResponseType(typeof(PaginatedPaymentResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(Roles = "Admin,Manager")] // Faqat admin va manager uchun
        public async Task<IActionResult> GetFilteredPayments([FromQuery] PaymentFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Filterlangan to'lovlarni olish so'rovi");
                var result = await _paymentService.GetFilteredPaymentsAsync(filter);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Noto'g'ri filter parametrlari: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Filterlangan to'lovlarni olishda xatolik");
                return StatusCode(500, "Filterlangan to'lovlarni olishda ichki server xatosi");
            }
        }

        /// <summary>
        /// To'lov holatini yangilash
        /// </summary>
        /// <param name="id">To'lov IDsi</param>
        /// <param name="paymentDto">Yangilanish ma'lumotlari</param>
        /// <returns>Yangilangan to'lov</returns>
        ///  PUT: https://localhost:7179/api/payment/update-status
        [HttpPut("update-status/{id}")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Authorize(Roles = "Admin,Manager")] // Faqat admin va manager uchun
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] PaymentUpdateDto paymentDto)
        {
            try
            {
                _logger.LogInformation("To'lov holatini yangilash so'rovi. ID: {PaymentId}", id);
                var result = await _paymentService.UpdatePaymentStatusAsync(id, paymentDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("To'lov topilmadi: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Noto'g'ri operatsiya: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "To'lov holatini yangilashda xatolik. ID: {PaymentId}", id);
                return StatusCode(500, "To'lov holatini yangilashda ichki server xatosi");
            }
        }

        /// <summary>
        /// To'lovni qaytarish (refund)
        /// </summary>
        /// <param name="id">To'lov IDsi</param>
        /// <param name="refundDto">Qaytarish ma'lumotlari</param>
        /// <returns>Yangilangan to'lov</returns>
        ///  POST: https://localhost:7179/api/payment/refund/id
        [HttpPost("refund/{id}")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Authorize(Roles = "Admin,Manager")] // Faqat admin va manager uchun
        public async Task<IActionResult> RefundPayment(int id, [FromBody] RefundRequestDto refundDto)
        {
            try
            {
                _logger.LogInformation("To'lovni qaytarish so'rovi. ID: {PaymentId}", id);
                var result = await _paymentService.RefundPaymentAsync(id, refundDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("To'lov topilmadi: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Noto'g'ri operatsiya: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (PaymentException ex)
            {
                _logger.LogError(ex, "To'lovni qaytarishda xatolik. ID: {PaymentId}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "To'lovni qaytarishda ichki xatolik. ID: {PaymentId}", id);
                return StatusCode(500, "To'lovni qaytarishda ichki server xatosi");
            }
        }

        /// <summary>
        /// To'lovni bekor qilish
        /// </summary>
        /// <param name="id">To'lov IDsi</param>
        /// <param name="reason">Bekor qilish sababi</param>
        /// <returns>Yangilangan to'lov</returns>
        ///  POST: https://localhost:7179/api/payment/cancel/id
        [HttpPost("cancel/{id}")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CancelPayment(int id, [FromBody] CancelPaymentRequestDto request)
        {
            try
            {
                _logger.LogInformation("To'lovni bekor qilish so'rovi. ID: {PaymentId}", id);
                var result = await _paymentService.CancelPaymentAsync(id, request.Reason);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("To'lov topilmadi: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Noto'g'ri operatsiya: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "To'lovni bekor qilishda xatolik. ID: {PaymentId}", id);
                return StatusCode(500, "To'lovni bekor qilishda ichki server xatosi");
            }
        }

        /// <summary>
        /// To'lov ma'lumotlarini yangilash
        /// </summary>
        /// <param name="id">To'lov IDsi</param>
        /// <param name="updateDto">Yangilanish ma'lumotlari</param>
        /// <returns>Yangilangan to'lov</returns>
        ///  PUT: https://localhost:7179/api/payment/fupdate-details/id
        [HttpPut("update-details/{id}")]
        [ProducesResponseType(typeof(PaymentResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UpdatePaymentDetails(int id, [FromBody] PaymentDetailsUpdateDto updateDto)
        {
            try
            {
                _logger.LogInformation("To'lov ma'lumotlarini yangilash so'rovi. ID: {PaymentId}", id);
                var result = await _paymentService.UpdatePaymentDetailsAsync(id, updateDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("To'lov topilmadi: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Noto'g'ri operatsiya: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "To'lov ma'lumotlarini yangilashda xatolik. ID: {PaymentId}", id);
                return StatusCode(500, "To'lov ma'lumotlarini yangilashda ichki server xatosi");
            }
        }

        /// <summary>
        /// Payment callback uchun endpoint (payment providerdan kelgan callback)
        /// </summary>
        /// <param name="callbackDto">Callback ma'lumotlari</param>
        /// <returns>Natija</returns>
        ///  POST: https://localhost:7179/api/payment/callback
        [HttpPost("callback")]
        [AllowAnonymous] // Payment provider uchun ochiq qoldirilgan
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ProcessPaymentCallback([FromBody] PaymentCallbackDto callbackDto)
        {
            try
            {
                _logger.LogInformation("Payment callback qabul qilindi. PaymentID: {PaymentId}", callbackDto.PaymentId);
                var result = await _paymentService.ProcessPaymentCallbackAsync(callbackDto);
                return Ok(result);
            }
            catch (SecurityException ex)
            {
                _logger.LogWarning("Xavfsizlik xatosi: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("To'lov topilmadi: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment callback ishlashda xatolik");
                return StatusCode(500, "Payment callback ishlashda ichki server xatosi");
            }
        }
    }
}
