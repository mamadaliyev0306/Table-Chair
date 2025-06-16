using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Faqat autentifikatsiyadan o'tgan foydalanuvchilar uchun
    public class WishlistItemController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistItemController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // POST: api/WishlistItem
        [HttpPost]
        public async Task<IActionResult> Add(int userId ,[FromBody] WishlistItemCreateDto dto)
        {
            await _wishlistService.AddAsync(userId, dto);
            return Ok(new { message = "Item added to wishlist" });
        }

        // DELETE: api/WishlistItem/{productId}
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Remove(int productId,int userId)
        {
            await _wishlistService.RemoveAsync(userId, productId);
            return Ok(new { message = "Item removed from wishlist" });
        }

        // GET: api/WishlistItem/exists/{productId}
        [HttpGet("exists/{productId:int}")]
        public async Task<IActionResult> Exists(int productId,int userId)
        {
            var exists = await _wishlistService.ExistsAsync(userId, productId);
            return Ok(exists);
        }

        // GET: api/WishlistItem/count
        [HttpGet("count/{userId}")]
        public async Task<IActionResult> GetCount(int userId)
        {
            var count = await _wishlistService.GetWishlistCountAsync(userId);
            return Ok(count);
        }

        // GET: api/WishlistItem/userId
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishlistByUser(int userId)
        {
            var wishlist = await _wishlistService.GetWishlistProductsAsync(userId);
            return Ok(wishlist);
        }

        // Fix for the CS0029 error in the Toggle method
        [HttpPost("toggle/{productId}/{userId}")]
        public async Task<IActionResult> Toggle(int productId,int userId)
        {
            try
            {
                var result = await _wishlistService.ToggleWishlistAsync(userId, productId);

                // Assuming result is of type WishlistToggleResultDto, use its IsInWishlist property
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = result.IsInWishlist ? "Mahsulot yoqtirilganlarga qo'shildi"
                                                  : "Mahsulot yoqtirilganlardan olib tashlandi"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ichki server xatosi",
                    detail = ex.Message
                });
            }
        }
    }
}

