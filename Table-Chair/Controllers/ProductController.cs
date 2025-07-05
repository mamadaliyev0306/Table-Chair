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
using Table_Chair_Application.Responses;

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

        [HttpGet("getall")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Barcha mahsulotlar so'rovi boshlandi.");
            var products = await _productService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products));
        }

        [HttpGet("getbyId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Id {Id} bilan mahsulot so'rovi.", id);
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound(ApiResponse<ProductDto>.FailResponse("Mahsulot topilmadi."));

            return Ok(ApiResponse<ProductDto>.SuccessResponse(product));
        }

        [HttpGet("getbycategory/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            _logger.LogInformation("Kategoriya {CategoryId} uchun mahsulotlar so'rovi.", categoryId);
            var products = await _productService.GetByCategoryAsync(categoryId);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products));
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public IActionResult Search(string searchTerm)
        {
            _logger.LogInformation("Mahsulot qidirilmoqda: {SearchTerm}", searchTerm);
            var result = _productService.SearchProduct(searchTerm);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(result));
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin,Seller")]
        [SwaggerRequestExample(typeof(CreateProductDto), typeof(CreateProductDtoExample))]
        public async Task<IActionResult> Add([FromBody] CreateProductDto productDto)
        {
            _logger.LogInformation("Yangi mahsulot qo'shish so'rovi.");
            await _productService.AddAsync(productDto);
            return Ok(ApiResponse<string>.SuccessResponse("", "Mahsulot muvaffaqiyatli qo'shildi."));
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin,Seller")]
        [SwaggerRequestExample(typeof(UpdateProductDto), typeof(UpdateProductDtoExample))]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto productDto)
        {
            _logger.LogInformation("Id {Id} bilan mahsulot yangilash so'rovi.", productDto.Id);
            await _productService.UpdateAsync(productDto);
            return Ok(ApiResponse<string>.SuccessResponse("", "Mahsulot muvaffaqiyatli yangilandi."));
        }

        [HttpPatch("update-stock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStock(int productId, int quantity)
        {
            _logger.LogInformation("Mahsulot Id {ProductId} stokini yangilash: {Quantity}.", productId, quantity);
            await _productService.UpdateStockAsync(productId, quantity);
            return Ok(ApiResponse<string>.SuccessResponse("", "Stok miqdori muvaffaqiyatli yangilandi."));
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Id {Id} bilan mahsulotni o'chirish so'rovi.", id);
            await _productService.DeleteAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("", "Mahsulot muvaffaqiyatli o'chirildi."));
        }

        [HttpPost("filter")]
        [AllowAnonymous]
        [SwaggerRequestExample(typeof(ProductFilterDto), typeof(ProductFilterDtoExample))]
        public async Task<IActionResult> GetFilteredProducts([FromBody] ProductFilterDto filterDto, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("Mahsulotlar filtrlanmoqda.");
            var paginatedList = await _productService.GetFilteredProductsAsync(filterDto, pageNumber, pageSize);

            // Convert PaginatedList<ProductDto> to PagedResponse<ProductDto>
            var pagedResponse = new PagedResponse<ProductDto>
            {
                Data = paginatedList.Items,
                PageNumber = paginatedList.PageNumber,
                PageSize = paginatedList.PageSize,
                TotalRecords = paginatedList.TotalCount,
                Success = true,
                Message = "Success"
            };

            return Ok(ApiResponse<PagedResponse<ProductDto>>.SuccessResponse(pagedResponse));
        }

        [HttpDelete("sofdelete/{id}")]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            await _productService.SoftDeleteAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("", "Mahsulot soft delete qilindi."));
        }

        [HttpGet("category/{categoryId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategory(int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId, page, pageSize);
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                result.Items,
                result.TotalPages,
                result.PageNumber,
                result.PageSize,
                result.TotalCount
            }));
        }

        [HttpGet("{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsWithWishlistInfoAsync(int userId)
        {
            var result = await _productService.GetProductsWithWishlistInfoAsync(userId);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(result));
        }
    }

}
