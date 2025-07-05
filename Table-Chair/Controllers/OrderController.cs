using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Table_Chair.Examples.BlogExample;
using Table_Chair.Examples.OrderExamples;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;

namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Buyurtma yaratish")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 201)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var order = await _orderService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, ApiResponse<OrderDto>.SuccessResponse(order));
        }

        [HttpPost("from-cart/{userId:int}")]
        [SwaggerOperation(Summary = "Foydalanuvchining savatchasidan buyurtma yaratish")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [SwaggerRequestExample(typeof(CreateOrderDto), typeof(CreateOrderDtoExample))]
        public async Task<IActionResult> CreateOrderFromCart(int userId, [FromBody] CheckoutDto dto)
        {
            var order = await _orderService.CreateOrderFromCartAsync(userId, dto);
            return Ok(ApiResponse<OrderDto>.SuccessResponse(order));
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Barcha buyurtmalarni olish (Admin)")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderDto>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(ApiResponse<List<OrderDto>>.SuccessResponse(orders.ToList()));
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Buyurtmani ID orqali olish")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            return Ok(ApiResponse<OrderDto?>.SuccessResponse(order));
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Buyurtmani tahrirlash")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [SwaggerRequestExample(typeof(OrderUpdateDto), typeof(OrderUpdateDtoExample))]
        public async Task<IActionResult> Update([FromBody] OrderUpdateDto dto)
        {
            await _orderService.UpdateAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Buyurtma yangilandi"));
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Buyurtmani o'chirish")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPatch("{orderId:int}/status")]
        [SwaggerOperation(Summary = "Buyurtma statusini yangilash")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromQuery] OrderStatus status)
        {
            await _orderService.UpdateOrderStatusAsync(orderId, status);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty,"Status muvaffaqiyatli yangilandi"));
        }

        [HttpPost("{orderId:int}/cancel")]
        [SwaggerOperation(Summary = "Buyurtmani bekor qilish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            await _orderService.CancelOrderAsync(orderId);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty,"Buyurtma bekor qilindi"));
        }

        [HttpGet("{orderId:int}/total")]
        [SwaggerOperation(Summary = "Buyurtma umumiy summasini hisoblash")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 200)]
        public async Task<IActionResult> GetTotalPrice(int orderId)
        {
            var total = await _orderService.GetTotalPriceAsync(orderId);
            return Ok(ApiResponse<decimal>.SuccessResponse(total));
        }

        [HttpGet("latest")]
        [SwaggerOperation(Summary = "Oxirgi buyurtmalar (Admin)")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderDto>>), 200)]
        public async Task<IActionResult> GetLatestOrders([FromQuery] int count = 10)
        {
            var latest = await _orderService.GetLatestOrdersAsync(count);
            return Ok(ApiResponse<List<OrderDto>>.SuccessResponse(latest.ToList()));
        }

        [HttpGet("date-range")]
        [SwaggerOperation(Summary = "Sana oralig'ida buyurtmalar (Admin)")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderDto>>), 200)]
        public IActionResult GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest(ApiResponse<string>.FailResponse("Boshlanish sanasi tugash sanasidan oldin bo'lishi kerak"));

            var result = _orderService.GetOrdersByDateRange(startDate, endDate);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(result));
        }

        [HttpGet("user/{userId:int}")]
        [SwaggerOperation(Summary = "Foydalanuvchining barcha buyurtmalari")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderDto>>), 200)]
        public IActionResult GetUserOrders(int userId)
        {
            var orders = _orderService.GetUserOrders(userId);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders));
        }
    }
}


