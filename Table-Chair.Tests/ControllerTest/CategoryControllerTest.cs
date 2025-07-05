using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;
using Xunit;

namespace Table_Chair.Tests.ControllerTests
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _mockService;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockService = new Mock<ICategoryService>();
            _controller = new CategoryController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfCategories()
        {
            var categories = new List<CategoryDto> { new CategoryDto { Id = 1, Name = "Test" } };
            _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<CategoryDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Single(response.Data);
        }

        [Fact]
        public async Task GetById_ReturnsCategory()
        {
            var category = new CategoryDto { Id = 1, Name = "Test" };
            _mockService.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<CategoryDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
        }

        [Fact]
        public async Task GetActiveCategories_ReturnsOnlyActive()
        {
            var list = new List<CategoryDto> { new CategoryDto { Id = 1, Name = "Test" } };
            _mockService.Setup(x => x.GetActiveCategoriesAsync()).ReturnsAsync(list);

            var result = await _controller.GetActiveCategories();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<CategoryDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
        }

        [Fact]
        public async Task GetWithProducts_ReturnsCategoriesWithProducts()
        {
            var list = new List<CategoryWithProductsDto> { new CategoryWithProductsDto { Id = 1, Name = "Test" } };
            _mockService.Setup(x => x.GetWithProductsAsync()).ReturnsAsync(list);

            var result = await _controller.GetWithProducts();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<CategoryWithProductsDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Single(response.Data);
        }

        [Fact]
        public async Task GetByName_ReturnsCategory()
        {
            var category = new CategoryDto { Id = 1, Name = "Test" };
            _mockService.Setup(x => x.GetByNameAsync("Test")).ReturnsAsync(category);

            var result = await _controller.GetByName("Test");
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<CategoryDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal("Test", response.Data.Name);
        }

        [Fact]
        public async Task Add_ReturnsCreatedCategory()
        {
            var dto = new CategoryCreateDto { Name = "New" };
            var created = new CategoryDto { Id = 2, Name = "New" };
            _mockService.Setup(x => x.AddAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Add(dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<ApiResponse<CategoryDto>>(createdResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal("New", response.Data.Name);
        }

        [Fact]
        public async Task Update_WhenSuccessful_ReturnsNoContent()
        {
            var dto = new CategoryUpdateDto { Id = 1, Name = "Updated" };
            _mockService.Setup(x => x.UpdateAsync(dto)).ReturnsAsync(true);

            var result = await _controller.Update(dto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_WhenNotFound_ReturnsNotFound()
        {
            var dto = new CategoryUpdateDto { Id = 99, Name = "X" };
            _mockService.Setup(x => x.UpdateAsync(dto)).ReturnsAsync(false);

            var result = await _controller.Update(dto);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(notFound.Value);
        }

        [Fact]
        public async Task Delete_WhenSuccessful_ReturnsNoContent()
        {
            _mockService.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WhenFailed_ReturnsBadRequest()
        {
            _mockService.Setup(x => x.DeleteAsync(99)).ReturnsAsync(false);
            var result = await _controller.Delete(99);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(badResult.Value);
        }

        [Fact]
        public async Task GetProductsByCategory_ReturnsPaged()
        {
            var paginatedProducts = new PaginatedList<ProductDto>(new List<ProductDto>(), 0, 1, 10);
            _mockService.Setup(x => x.GetProductsByCategoryAsync(1, 1, 10)).ReturnsAsync(paginatedProducts);

            var result = await _controller.GetProductsByCategory(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetByType_ReturnsFilteredCategories()
        {
            var categories = new List<CategoryDto> { new CategoryDto { Id = 1, Name = "Type1" } };
            _mockService.Setup(x => x.GetByTypeAsync(CategoryType.Product)).ReturnsAsync(categories);

            var result = await _controller.GetByType(CategoryType.Product);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<CategoryDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Single(response.Data);
        }
    }
}
