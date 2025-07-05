using Microsoft.AspNetCore.Mvc;
using Moq;
using Table_Chair_Api.Controllers;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Tests.ControllerTest
{
    public class WishlistItemControllerTests
    {
        private readonly Mock<IWishlistService> _wishlistServiceMock;
        private readonly WishlistItemController _controller;

        public WishlistItemControllerTests()
        {
            _wishlistServiceMock = new Mock<IWishlistService>();
            _controller = new WishlistItemController(_wishlistServiceMock.Object);
        }

        [Fact]
        public async Task Add_ReturnsOk()
        {
            var userId = 1;
            var dto = new WishlistItemCreateDto { ProductId = 10 };

            var result = await _controller.Add(userId, dto) as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ApiResponse<string>>(result.Value);
            Assert.True(response.Success);
            Assert.Equal("Mahsulot yoqtirilganlarga qo‘shildi", response.Message);
        }

        [Fact]
        public async Task Remove_ReturnsOk()
        {
            var userId = 1;
            var productId = 10;

            var result = await _controller.Remove(userId, productId) as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ApiResponse<string>>(result.Value);
            Assert.True(response.Success);
            Assert.Equal("Mahsulot yoqtirilganlardan o‘chirildi", response.Message);
        }

        [Fact]
        public async Task Exists_ReturnsOk()
        {
            var userId = 1;
            var productId = 10;

            _wishlistServiceMock.Setup(s => s.ExistsAsync(userId, productId)).ReturnsAsync(true);

            var result = await _controller.Exists(userId, productId) as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ApiResponse<bool>>(result.Value);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task GetCount_ReturnsOk()
        {
            var userId = 1;
            _wishlistServiceMock.Setup(s => s.GetWishlistCountAsync(userId)).ReturnsAsync(5);

            var result = await _controller.GetCount(userId) as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ApiResponse<int>>(result.Value);
            Assert.Equal(5, response.Data);
        }

        [Fact]
        public async Task GetWishlistByUser_ReturnsOk()
        {
            var userId = 1;
            var list = new List<WishlistItemDto> { new WishlistItemDto { ProductId = 10 } };
            _wishlistServiceMock.Setup(s => s.GetWishlistProductsAsync(userId)).ReturnsAsync(list);

            var result = await _controller.GetWishlistByUser(userId) as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ApiResponse<List<WishlistItemDto>>>(result.Value);
            Assert.NotNull(response.Data);
            Assert.Single(response.Data);
        }

        [Fact]
        public async Task Toggle_ReturnsOk_WithCorrectMessage()
        {
            var userId = 1;
            var productId = 10;

            _wishlistServiceMock.Setup(s => s.ToggleWishlistAsync(userId, productId))
                .ReturnsAsync(new WishlistToggleResultDto { IsInWishlist = true });

            var result = await _controller.Toggle(userId, productId) as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ApiResponse<object>>(result.Value);
            Assert.True(response.Success);
            Assert.Equal("Mahsulot yoqtirilganlarga qo‘shildi", response.Message);
        }
    }
}

