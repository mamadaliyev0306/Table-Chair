using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Entity.Enums;
using Microsoft.AspNetCore.Http;
using SendGrid.Helpers.Errors.Model;
using Microsoft.AspNetCore.Authorization;

namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Produces("application/json")]
    [Authorize] // Faqat autentifikatsiyadan o'tgan foydalanuvchilar uchun
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="createOrderDto">Order creation data</param>
        /// <returns>Created order</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                var order = await _orderService.AddAsync(createOrderDto);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates order from user's cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="checkoutDto">Checkout data</param>
        /// <returns>Created order</returns>
        [HttpPost("from-cart/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateOrderFromCart(int userId, [FromBody] CheckoutDto checkoutDto)
        {
            try
            {
                var order = await _orderService.CreateOrderFromCartAsync(userId, checkoutDto);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User or cart not found");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order from cart");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Gets all orders
        /// </summary>
        /// <returns>List of orders</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Gets order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found");
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="orderDto">Order data</param>
        /// <returns>Updated order</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDto orderDto)
        {
            try
            {
                if (id != orderDto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                await _orderService.UpdateAsync(orderDto);
                return Ok(new { message = "Order updated successfully" });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes order
        /// </summary>
        /// <param name="id">Order ID</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _orderService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found");
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates order status
        /// </summary>
        [HttpPatch("{orderId:int}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromQuery] OrderStatus status)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, status);
                return Ok(new { message = "Order status updated successfully" });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{orderId:int}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                await _orderService.CancelOrderAsync(orderId);
                return Ok(new { message = "Order cancelled successfully" });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Order already cancelled");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{orderId:int}/total")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTotalPrice(int orderId)
        {
            try
            {
                var totalPrice = await _orderService.GetTotalPriceAsync(orderId);
                return Ok(new { totalPrice });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found");
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLatestOrders([FromQuery] int count = 10)
        {
            var latestOrders = await _orderService.GetLatestOrdersAsync(count);
            return Ok(latestOrders);
        }
        [HttpGet("date-range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest(new { message = "Start date must be before end date" });

                var orders = _orderService.GetOrdersByDateRange(startDate, endDate);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders by date range");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetUserOrders(int userId)
        {
            try
            {
                var orders = _orderService.GetUserOrders(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user orders");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

