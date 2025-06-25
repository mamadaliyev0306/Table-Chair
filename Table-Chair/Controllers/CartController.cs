using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Responses;
using Table_Chair.Examples.BlogExample;
using Table_Chair_Application.Dtos.BlogDtos;

namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet("user/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<List<CartDto>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetUserCarts(int userId)
        {
            var result = await _cartService.GetUserCartsAsync(userId);
            return Ok(ApiResponse<List<CartDto>>.SuccessResponse(result.ToList()));
        }
        [HttpPost("user/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), 200)]
        public async Task<IActionResult> CreateCart(int userId)
        {
            var result = await _cartService.CreateCartForUserAsync(userId);
            return Ok(ApiResponse<CartDto>.SuccessResponse(result, "Savatcha yaratildi"));
        }

        [HttpGet("active/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetActiveCart(int userId)
        {
            var result = await _cartService.GetCartByUserIdAsync(userId);
            return Ok(ApiResponse<CartDto?>.SuccessResponse(result));
        }
        [HttpGet("{cartId}/items")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<List<CartItemDto>>), 200)]
        public async Task<IActionResult> GetCartItems(int cartId)
        {
            var result = await _cartService.GetCartItemsAsync(cartId);
            return Ok(ApiResponse<List<CartItemDto>>.SuccessResponse(result));
        }

        [HttpPost("{cartId}/items")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> AddItemToCart(int cartId, [FromBody] CartItemCreateDto itemDto)
        {
            await _cartService.AddItemToCartAsync(cartId, itemDto);
            return Ok(ApiResponse<string>.SuccessResponse("Maxsulot savatchaga qo'shildi"));
        }

        [HttpPut("items/{cartItemId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> UpdateItemQuantity(int cartItemId, [FromQuery] int newQuantity)
        {
            await _cartService.UpdateCartItemQuantityAsync(cartItemId, newQuantity);
            return Ok(ApiResponse<string>.SuccessResponse("Miqdor muvaffaqiyatli yangilandi"));
        }

        [HttpDelete("items/{cartItemId}")]
        [Authorize]
        [ProducesResponseType(204)]
        public async Task<IActionResult> RemoveItemFromCart(int cartItemId)
        {
            await _cartService.RemoveItemFromCartAsync(cartItemId);
            return NoContent();
        }

        [HttpDelete("{cartId}/clear")]
        [Authorize]
        [ProducesResponseType(204)]
        public async Task<IActionResult> ClearCart(int cartId)
        {
            await _cartService.ClearCartAsync(cartId);
            return NoContent();
        }
        [HttpGet("user/{userId}/exists")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> CartExists(int userId)
        {
            var result = await _cartService.CartExistsAsync(userId);
            return Ok(ApiResponse<bool>.SuccessResponse(result));
        }
        [HttpGet("{cartId}/total")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 200)]
        public async Task<IActionResult> CalculateCartTotal(int cartId)
        {
            var total = await _cartService.CalculateCartTotalAsync(cartId);
            return Ok(ApiResponse<decimal>.SuccessResponse(total));
        }

        [HttpGet("{cartId}/item-count")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        public async Task<IActionResult> GetCartItemCount(int cartId)
        {
            var count = await _cartService.GetCartItemCountAsync(cartId);
            return Ok(ApiResponse<int>.SuccessResponse(count));
        }
    }
}
