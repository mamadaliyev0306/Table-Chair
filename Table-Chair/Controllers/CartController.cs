using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
   // [Authorize] // Faqat autentifikatsiyadan o'tgan foydalanuvchilar uchun
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserCarts(int userId)
        {
            try
            {
                var carts = await _cartService.GetUserCartsAsync(userId);
                return Ok(carts);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user ID: {UserId}", userId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting carts for user {UserId}", userId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }


        [HttpPost("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCart(int userId)
        {
            try
            {
                var cart = await _cartService.CreateCartForUserAsync(userId);
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user ID: {UserId}", userId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart for user {UserId}", userId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpGet("active/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveCart(int userId)
        {
            try
            {
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    return NotFound(new { Message = "No active cart found" });
                }
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user ID: {UserId}", userId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active cart for user {UserId}", userId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpGet("{cartId}/items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCartItems(int cartId)
        {
            try
            {
                var items = await _cartService.GetCartItemsAsync(cartId);
                return Ok(items);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid cart ID: {CartId}", cartId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Cart not found: {CartId}", cartId);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting items for cart {CartId}", cartId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpPost("{cartId}/items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddItemToCart(int cartId, [FromBody] CartItemCreateDto itemDto)
        {
            try
            {
                await _cartService.AddItemToCartAsync(cartId, itemDto);
                return Ok(new { Message = "Item added to cart successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request data for cart {CartId}", cartId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Cart or product not found for cart {CartId}", cartId);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart {CartId}", cartId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpPut("items/{cartItemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItemQuantity(int cartItemId, [FromQuery] int newQuantity)
        {
            try
            {
                await _cartService.UpdateCartItemQuantityAsync(cartItemId, newQuantity);
                return Ok(new { Message = "Item quantity updated successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid quantity for item {CartItemId}", cartItemId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Item not found: {CartItemId}", cartItemId);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity for item {CartItemId}", cartItemId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpDelete("items/{cartItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItemFromCart(int cartItemId)
        {
            try
            {
                await _cartService.RemoveItemFromCartAsync(cartItemId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid item ID: {CartItemId}", cartItemId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Item not found: {CartItemId}", cartItemId);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item {CartItemId}", cartItemId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpDelete("{cartId}/clear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearCart(int cartId)
        {
            try
            {
                await _cartService.ClearCartAsync(cartId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid cart ID: {CartId}", cartId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Cart not found: {CartId}", cartId);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart {CartId}", cartId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }


        [HttpGet("user/{userId}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CartExists(int userId)
        {
            try
            {
                var exists = await _cartService.CartExistsAsync(userId);
                return Ok(new { Exists = exists });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user ID: {UserId}", userId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cart existence for user {UserId}", userId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }


        [HttpGet("{cartId}/total")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CalculateCartTotal(int cartId)
        {
            try
            {
                var total = await _cartService.CalculateCartTotalAsync(cartId);
                return Ok(new { Total = total });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid cart ID: {CartId}", cartId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Cart not found: {CartId}", cartId);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total for cart {CartId}", cartId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpGet("{cartId}/item-count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCartItemCount(int cartId)
        {
            try
            {
                var count = await _cartService.GetCartItemCountAsync(cartId);
                return Ok(new { Count = count });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid cart ID: {CartId}", cartId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Cart not found: {CartId}", cartId);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item count for cart {CartId}", cartId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }
    }
}