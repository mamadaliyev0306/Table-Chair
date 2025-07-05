using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Xunit;

namespace Table_Chair.Tests.ControllerTest
{
    public class CartControllerTests
    {
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _cartServiceMock = new Mock<ICartService>();
            _controller = new CartController(_cartServiceMock.Object);
        }

        [Fact]
        public async Task GetUserCarts_ReturnsCarts()
        {
            var carts = new List<CartDto> { new CartDto { Id = 1, UserId = 1 } };
            _cartServiceMock.Setup(x => x.GetUserCartsAsync(1)).ReturnsAsync(carts);

            var result = await _controller.GetUserCarts(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<CartDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);
        }

        [Fact]
        public async Task CreateCart_ReturnsCreatedCart()
        {
            var cart = new CartDto { Id = 1, UserId = 1 };
            _cartServiceMock.Setup(x => x.CreateCartForUserAsync(1)).ReturnsAsync(cart);

            var result = await _controller.CreateCart(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<CartDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.UserId);
        }

        [Fact]
        public async Task GetActiveCart_ReturnsCart()
        {
            var cart = new CartDto { Id = 1, UserId = 1 };
            _cartServiceMock.Setup(x => x.GetCartByUserIdAsync(1)).ReturnsAsync(cart);

            var result = await _controller.GetActiveCart(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<CartDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.UserId);
        }

        [Fact]
        public async Task AddItemToCart_ReturnsSuccess()
        {
            var dto = new CartItemCreateDto { ProductId = 1, Quantity = 2 };
            _cartServiceMock.Setup(x => x.AddItemToCartAsync(1, dto)).Returns(Task.CompletedTask);

            var result = await _controller.AddItemToCart(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task UpdateItemQuantity_ReturnsSuccess()
        {
            _cartServiceMock.Setup(x => x.UpdateCartItemQuantityAsync(1, 5)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateItemQuantity(1, 5);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task RemoveItemFromCart_ReturnsNoContent()
        {
            _cartServiceMock.Setup(x => x.RemoveItemFromCartAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.RemoveItemFromCart(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ClearCart_ReturnsNoContent()
        {
            _cartServiceMock.Setup(x => x.ClearCartAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.ClearCart(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CartExists_ReturnsTrue()
        {
            _cartServiceMock.Setup(x => x.CartExistsAsync(1)).ReturnsAsync(true);

            var result = await _controller.CartExists(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task CalculateCartTotal_ReturnsTotal()
        {
            _cartServiceMock.Setup(x => x.CalculateCartTotalAsync(1)).ReturnsAsync(99.99m);

            var result = await _controller.CalculateCartTotal(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<decimal>>(okResult.Value);
            Assert.Equal(99.99m, response.Data);
        }

        [Fact]
        public async Task GetCartItemCount_ReturnsCount()
        {
            _cartServiceMock.Setup(x => x.GetCartItemCountAsync(1)).ReturnsAsync(3);

            var result = await _controller.GetCartItemCount(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<int>>(okResult.Value);
            Assert.Equal(3, response.Data);
        }
    }
}