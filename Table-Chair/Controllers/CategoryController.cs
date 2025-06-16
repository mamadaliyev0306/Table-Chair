using Microsoft.AspNetCore.Mvc;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: https://localhost:7179/api/category/getall
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // GET: https://localhost:7179/api/category/getbyId/id
        [HttpGet("getbyId/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                return Ok(category);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // GET: https://localhost:7179/api/category/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCategories()
        {
            try
            {
                var categories = await _categoryService.GetActiveCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // GET: https://localhost:7179/api/category/with-products
        [HttpGet("with-products")]
        public async Task<IActionResult> GetWithProducts()
        {
            try
            {
                var categories = await _categoryService.GetWithProductsAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // GET: https://localhost:7179/api/category/getbyname/name
        [HttpGet("getbyname/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var category = await _categoryService.GetByNameAsync(name);
                return Ok(category);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // POST: https://localhost:7179/api/category/add
        [HttpPost("add")]
        public async Task<IActionResult> Add(CategoryCreateDto categoryCreateDto)
        {
            try
            {
                var createdCategory = await _categoryService.AddAsync(categoryCreateDto);
                return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // PUT: https://localhost:7179/api/category/update/id
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, CategoryDto categoryDto)
        {
            try
            {
                if (id != categoryDto.Id)
                    return BadRequest("Id does not match");

                var updated = await _categoryService.UpdateAsync(categoryDto);
                if (!updated)
                    return NotFound("Category not found");

                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // DELETE: https://localhost:7179/api/category/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _categoryService.DeleteAsync(id);
                if (!deleted)
                    return BadRequest("Category not found or has products");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

