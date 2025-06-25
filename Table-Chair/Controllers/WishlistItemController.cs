using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistItemController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistItemController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost("{userId}")]
        [SwaggerOperation(Summary = "Yoqtirgan mahsulotlar ro'yxatiga mahsulot qo'shish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Add(int userId, [FromBody] WishlistItemCreateDto dto)
        {
            await _wishlistService.AddAsync(userId, dto);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Mahsulot yoqtirilganlarga qo‘shildi"));
        }

        [HttpDelete("{userId}/{productId}")]
        [SwaggerOperation(Summary = "Yoqtirgan mahsulotlar ro'yxatidan mahsulotni o'chirish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Remove(int userId, int productId)
        {
            await _wishlistService.RemoveAsync(userId, productId);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Mahsulot yoqtirilganlardan o‘chirildi"));
        }

        [HttpGet("exists/{userId}/{productId}")]
        [SwaggerOperation(Summary = "Mahsulot yoqtirilganlarda mavjudligini tekshirish")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Exists(int userId, int productId)
        {
            var exists = await _wishlistService.ExistsAsync(userId, productId);
            return Ok(ApiResponse<bool>.SuccessResponse(exists));
        }

        [HttpGet("count/{userId}")]
        [SwaggerOperation(Summary = "Foydalanuvchining barcha yoqtirilgan mahsulotlari sonini olish")]
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        public async Task<IActionResult> GetCount(int userId)
        {
            var count = await _wishlistService.GetWishlistCountAsync(userId);
            return Ok(ApiResponse<int>.SuccessResponse(count));
        }

        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Foydalanuvchining barcha yoqtirilgan mahsulotlarini olish")]
        [ProducesResponseType(typeof(ApiResponse<List<WishlistItemDto>>), 200)]
        public async Task<IActionResult> GetWishlistByUser(int userId)
        {
            var wishlist = await _wishlistService.GetWishlistProductsAsync(userId);
            return Ok(ApiResponse<List<WishlistItemDto>>.SuccessResponse(wishlist));
        }

        [HttpPost("toggle/{userId}/{productId}")]
        [SwaggerOperation(Summary = "Yoqtirganlar ro'yxatida mahsulotni yoqish yoki olib tashlash")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Toggle(int userId, int productId)
        {
            var result = await _wishlistService.ToggleWishlistAsync(userId, productId);
            var message = result.IsInWishlist
                ? "Mahsulot yoqtirilganlarga qo‘shildi"
                : "Mahsulot yoqtirilganlardan olib tashlandi";

            return Ok(ApiResponse<object>.SuccessResponse(result, message));
        }
    }
}


