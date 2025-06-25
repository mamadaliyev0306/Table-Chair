using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Filters;
using Table_Chair.Examples.BlogExample;
using Table_Chair_Application.Dtos.BlogDtos;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Barcha mahsulotlarni olish
        /// </summary>
        [HttpGet("getall")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Barcha mahsulotlar so'rovi boshlandi.");
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        /// <summary>
        /// Mahsulotni ID orqali olish
        /// </summary>
        [HttpGet("getbyId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Id {Id} bilan mahsulot so'rovi.", id);
            var product = await _productService.GetByIdAsync(id);
            return Ok(product);
        }

        /// <summary>
        /// Mahsulotlarni kategoriya bo'yicha olish
        /// </summary>
        [HttpGet("getbycategory/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            _logger.LogInformation("Kategoriya {CategoryId} uchun mahsulotlar so'rovi.", categoryId);
            var products = await _productService.GetByCategoryAsync(categoryId);
            return Ok(products);
        }

        /// <summary>
        /// Mahsulotlarni izlash
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public IActionResult Search(string searchTerm)
        {
            _logger.LogInformation("Mahsulot qidirilmoqda: {SearchTerm}", searchTerm);
            var result = _productService.SearchProduct(searchTerm);
            return Ok(result);
        }

        /// <summary>
        /// Yangi mahsulot qo'shish
        /// </summary>
        [HttpPost("create")]
        [Authorize(Roles = "Admin,Seller")]
        [SwaggerRequestExample(typeof(CreateProductDto), typeof(CreateProductDtoExample))]
        public async Task<IActionResult> Add([FromBody] CreateProductDto productDto)
        {
            _logger.LogInformation("Yangi mahsulot qo'shish so'rovi.");
            await _productService.AddAsync(productDto);
            return Ok(new { Message = "Mahsulot muvaffaqiyatli qo'shildi." });
        }

        /// <summary>
        /// Mahsulotni yangilash
        /// </summary>
        [HttpPut("update")]
        [Authorize(Roles = "Admin,Seller")]
        [SwaggerRequestExample(typeof(UpdateProductDto), typeof(UpdateProductDtoExample))]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto productDto)
        {
            _logger.LogInformation("Id {Id} bilan mahsulot yangilash so'rovi.", productDto.Id);
            await _productService.UpdateAsync(productDto);
            return Ok(new { Message = "Mahsulot muvaffaqiyatli yangilandi." });
        }

        /// <summary>
        /// Stok miqdorini yangilash
        /// </summary>
        [HttpPatch("update-stock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStock(int productId, int quantity)
        {
            _logger.LogInformation("Mahsulot Id {ProductId} stokini yangilash: {Quantity}.", productId, quantity);
            await _productService.UpdateStockAsync(productId, quantity);
            return Ok(new { Message = "Stok miqdori muvaffaqiyatli yangilandi." });
        }

        /// <summary>
        /// Mahsulotni o'chirish
        /// </summary>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Id {Id} bilan mahsulotni o'chirish so'rovi.", id);
            await _productService.DeleteAsync(id);
            return Ok(new { Message = "Mahsulot muvaffaqiyatli o'chirildi." });
        }

        /// <summary>
        /// Mahsulotlarni filter bo'yicha olish
        /// </summary>
        [HttpPost("filter")]
        [AllowAnonymous]
        [SwaggerRequestExample(typeof(ProductFilterDto), typeof(ProductFilterDtoExample))]
        public async Task<IActionResult> GetFilteredProducts([FromBody] ProductFilterDto filterDto, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("Mahsulotlar filtrlanmoqda.");
            var products = await _productService.GetFilteredProductsAsync(filterDto, pageNumber, pageSize);
            return Ok(products);
        }

        /// <summary>
        /// Mahsulotni soft delete qilish
        /// </summary>
        [HttpDelete("sofdelete/{id}")]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            await _productService.SoftDeleteAsync(id);
            return Ok("Malumot o'chirildi");
        }

        /// <summary>
        /// Paged kategoriya bo'yicha mahsulotlar
        /// </summary>
        [HttpGet("category/{categoryId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategory(int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId, page, pageSize);
            return Ok(new
            {
                Products = result.Items,
                TotalPages = result.TotalPages,
                CurrentPage = result.PageNumber,
                PageSize = result.PageSize,
                TotalItems = result.TotalCount
            });
        }

        /// <summary>
        /// Foydalanuvchiga wishlist holati bilan mahsulotlar
        /// </summary>
        [HttpGet("{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsWithWishlistInfoAsync(int userId)
        {
            var result = await _productService.GetProductsWithWishlistInfoAsync(userId);
            return Ok(result);
        }
    }
}
