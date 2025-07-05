using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Tests.ControllerTest
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly Mock<ILogger<ProductController>> _mockLogger;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithProductList()
        {
            var products = new List<ProductDto> { new ProductDto { Id = 1, Name = "Test" } };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductDto>>>(ok.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenProductExists()
        {
            var product = new ProductDto { Id = 1, Name = "Test" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<ProductDto>>(ok.Value);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenProductNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ProductDto?)null);

            var result = await _controller.GetById(99);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<ProductDto>>(notFound.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenValidProduct()
        {
            var dto = new CreateProductDto
            {
                Name = "New Product",
                Price = 100,
                StockQuantity = 10,
                CategoryId = 1
            };

            var result = await _controller.Add(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(ok.Value);
            Assert.True(response.Success);
            Assert.Equal("Mahsulot muvaffaqiyatli qo'shildi.", response.Message);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenProductDeleted()
        {
            var result = await _controller.Delete(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(ok.Value);
            Assert.True(response.Success);
            Assert.Equal("Mahsulot muvaffaqiyatli o'chirildi.", response.Message);
        }
    }
}
