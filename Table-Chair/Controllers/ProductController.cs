using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Services.InterfaceServices;

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
        // GET: https://localhost:7179/api/product/getall
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Barcha mahsulotlar so'rovi boshlandi.");
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        // GET: https://localhost:7179/api/product/getbyId
        [HttpGet("getbyId/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Id {Id} bilan mahsulot so'rovi.", id);
            var product = await _productService.GetByIdAsync(id);
            return Ok(product);
        }
        // GET: https://localhost:7179/api/product/getbycategory/categoryId
        [HttpGet("getbycategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            _logger.LogInformation("Kategoriya {CategoryId} uchun mahsulotlar so'rovi.", categoryId);
            var products = await _productService.GetByCategoryAsync(categoryId);
            return Ok(products);
        }
        // GET: https://localhost:7179/api/product/search
        [HttpGet("search")]
        public IActionResult Search(string searchTerm)
        {
            _logger.LogInformation("Mahsulot qidirilmoqda: {SearchTerm}", searchTerm);
            var result = _productService.SearchProduct(searchTerm);
            return Ok(result);
        }
        // POST: https://localhost:7179/api/product/create
        [HttpPost("create")]
        public async Task<IActionResult> Add([FromBody] CreateProductDto productDto)
        {
            _logger.LogInformation("Yangi mahsulot qo'shish so'rovi.");
            await _productService.AddAsync(productDto);
            return Ok(new { Message = "Mahsulot muvaffaqiyatli qo'shildi." });
        }
        // PUT: https://localhost:7179/api/product/update
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto productDto)
        {
            _logger.LogInformation("Id {Id} bilan mahsulot yangilash so'rovi.", productDto.Id);
            await _productService.UpdateAsync(productDto);
            return Ok(new { Message = "Mahsulot muvaffaqiyatli yangilandi." });
        }
        // PATCH: https://localhost:7179/api/product/update-stock
        [HttpPatch("update-stock")]
        public async Task<IActionResult> UpdateStock(int productId, int quantity)
        {
            _logger.LogInformation("Mahsulot Id {ProductId} stokini yangilash: {Quantity}.", productId, quantity);
            await _productService.UpdateStockAsync(productId, quantity);
            return Ok(new { Message = "Stok miqdori muvaffaqiyatli yangilandi." });
        }
        // DELETE: https://localhost:7179/api/product/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Id {Id} bilan mahsulotni o'chirish so'rovi.", id);
            await _productService.DeleteAsync(id);
            return Ok(new { Message = "Mahsulot muvaffaqiyatli o'chirildi." });
        }
        // Post: https://localhost:7179/api/product/filter
        [HttpPost("filter")]
        public async Task<IActionResult> GetFilteredProducts([FromBody] ProductFilterDto filterDto, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("Mahsulotlar filtrlanmoqda.");
            var products = await _productService.GetFilteredProductsAsync(filterDto, pageNumber, pageSize);
            return Ok(products);
        }
        //Delete :https://localhost:7179/api/product/sofdelete/id
        [HttpDelete("sofdelete/{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            try
            {
                await _productService.SoftDeleteAsync(id);
                return Ok("malumot o'chirildi ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        // GET: api/products/category/{categoryId}?page=1&pageSize=20
        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(
            int categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
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
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProductsWithWishlistInfoAsync(int userId)
        {
            try
            {
                var result = await _productService.GetProductsWithWishlistInfoAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex+  Environment.NewLine + ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }
    }
}
