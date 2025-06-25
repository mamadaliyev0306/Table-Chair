using Microsoft.AspNetCore.Mvc;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Application.Responses;
using Swashbuckle.AspNetCore.Filters;
using Table_Chair.Examples.CategoryExample;
using Microsoft.AspNetCore.Authorization;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Barcha kategoriyalar ro'yxatini qaytaradi
        /// </summary>
        [HttpGet("getall")]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(categories.ToList()));
        }

        /// <summary>
        /// Berilgan ID bo‘yicha kategoriya qaytaradi
        /// </summary>
        [HttpGet("getbyid/{id}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return Ok(ApiResponse<CategoryDto?>.SuccessResponse(category));
        }

        /// <summary>
        /// Faqat aktiv kategoriyalarni qaytaradi
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveCategories()
        {
            var result = await _categoryService.GetActiveCategoriesAsync();
            return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(result.ToList()));
        }

        /// <summary>
        /// Har bir kategoriya bilan birga unga tegishli mahsulotlar ro‘yhatini qaytaradi
        /// </summary>
        [HttpGet("with-products")]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryWithProductsDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWithProducts()
        {
            var result = await _categoryService.GetWithProductsAsync();
            return Ok(ApiResponse<List<CategoryWithProductsDto>>.SuccessResponse(result.ToList()));
        }

        /// <summary>
        /// Kategoriya nomi bo‘yicha ma’lumotni qaytaradi
        /// </summary>
        [HttpGet("getbyname/{name}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByName(string name)
        {
            var category = await _categoryService.GetByNameAsync(name);
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
        }

        /// <summary>
        /// Yangi kategoriya qo‘shadi
        /// </summary>
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(CategoryCreateDtoExample), typeof(CategoryCreateDtoExample))]
        public async Task<IActionResult> Add([FromBody] CategoryCreateDto dto)
        {
            var createdCategory = await _categoryService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, ApiResponse<CategoryDto>.SuccessResponse(createdCategory, "Kategoriya yaratildi"));
        }

        /// <summary>
        /// Kategoriya ma’lumotlarini yangilaydi
        /// </summary>
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [SwaggerRequestExample(typeof(CategoryUpdateDtoExample), typeof(CategoryUpdateDtoExample))]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateDto dto)
        {

            var updated = await _categoryService.UpdateAsync(dto);
            if (!updated)
                return NotFound(ErrorResponse.Create("Kategoriya topilmadi"));

            return NoContent();
        }

        /// <summary>
        /// Kategoriya o‘chiradi
        /// </summary>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);
            if (!deleted)
                return BadRequest(ErrorResponse.Create("Kategoriya topilmadi yoki unga tegishli mahsulotlar mavjud"));

            return NoContent();
        }
        [HttpGet("{categoryId}/products")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page number and size must be greater than zero.");

            var pagedProducts = await _categoryService.GetProductsByCategoryAsync(categoryId, pageNumber, pageSize);

            return Ok(pagedProducts);
        }
    }
}


