using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Table_Chair.Controllers;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Entity.Enums;
using Table_Chair_Application.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class OrderControllerTests
{
    private readonly OrderController _controller;
    private readonly Mock<IOrderService> _mockService = new();

    public OrderControllerTests()
    {
        _controller = new OrderController(_mockService.Object);
    }

    [Fact]
    public async Task CreateOrder_ReturnsCreatedResponse()
    {
        var dto = new CreateOrderDto();
        var orderDto = new OrderDto { Id = 1 };
        _mockService.Setup(s => s.AddAsync(dto)).ReturnsAsync(orderDto);

        var result = await _controller.CreateOrder(dto);

        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        var response = Assert.IsType<ApiResponse<OrderDto>>(createdAtResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task CreateOrderFromCart_ReturnsOk()
    {
        int userId = 1;
        var dto = new CheckoutDto();
        var orderDto = new OrderDto { Id = 2 };
        _mockService.Setup(s => s.CreateOrderFromCartAsync(userId, dto)).ReturnsAsync(orderDto);

        var result = await _controller.CreateOrderFromCart(userId, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<OrderDto>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsOrderList()
    {
        var orders = new List<OrderDto> { new() { Id = 1 }, new() { Id = 2 } };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<OrderDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count);
    }

    [Fact]
    public async Task GetById_ReturnsOrder()
    {
        var order = new OrderDto { Id = 5 };
        _mockService.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(order);

        var result = await _controller.GetById(5);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<OrderDto>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(5, response.Data.Id);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var dto = new OrderUpdateDto();
        _mockService.Setup(s => s.UpdateAsync(dto)).Returns(Task.CompletedTask);

        var result = await _controller.Update(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        _mockService.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsOk()
    {
        _mockService.Setup(s => s.UpdateOrderStatusAsync(1, OrderStatus.Delivered)).Returns(Task.CompletedTask);

        var result = await _controller.UpdateStatus(1, OrderStatus.Delivered);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
        Assert.Equal("Status muvaffaqiyatli yangilandi", response.Message);
    }

    [Fact]
    public async Task CancelOrder_ReturnsOk()
    {
        _mockService.Setup(s => s.CancelOrderAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.CancelOrder(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
        Assert.Equal("Buyurtma bekor qilindi", response.Message);
    }

    [Fact]
    public async Task GetTotalPrice_ReturnsTotal()
    {
        _mockService.Setup(s => s.GetTotalPriceAsync(1)).ReturnsAsync(99.99m);

        var result = await _controller.GetTotalPrice(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<decimal>>(okResult.Value);
        Assert.Equal(99.99m, response.Data);
    }

    [Fact]
    public void GetByDateRange_ReturnsRangeOrders()
    {
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);
        var list = new List<OrderDto> { new() { Id = 1 } };
        _mockService.Setup(s => s.GetOrdersByDateRange(from, to)).Returns(list.AsQueryable());

        var result = _controller.GetByDateRange(from, to);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<IEnumerable<OrderDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
    }

    [Fact]
    public void GetUserOrders_ReturnsUserOrders()
    {
        int userId = 3;
        var list = new List<OrderDto> { new() { Id = 7 } };
        _mockService.Setup(s => s.GetUserOrders(userId)).Returns(list.AsQueryable());

        var result = _controller.GetUserOrders(userId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<IEnumerable<OrderDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
    }
}

