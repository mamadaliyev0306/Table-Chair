using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos.ShippingAddressDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;

namespace Table_Chair.Tests.ControllerTest
{
    public class ShippingAddressControllerTests
    {
        private readonly Mock<IShippingAddressService> _mockService;
        private readonly Mock<ILogger<ShippingAddressController>> _mockLogger;
        private readonly ShippingAddressController _controller;

        public ShippingAddressControllerTests()
        {
            _mockService = new Mock<IShippingAddressService>();
            _mockLogger = new Mock<ILogger<ShippingAddressController>>();
            _controller = new ShippingAddressController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenValidDto()
        {
            var dto = new ShippingAddressCreateDto
            {
                RecipientName = "Ali",
                PhoneNumber = "998901234567",
                AddressLine = "Ko‘cha 1",
                City = "Toshkent",
                Region = "Yunusobod",
                PostalCode = "100011",
                Country = CountryMethod.Uzbekistan
            };

            var result = await _controller.Create(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenDtoIsNull()
        {
            var dto = (ShippingAddressCreateDto?)null;

            var result = await _controller.Create(dto!);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(bad.Value);
            Assert.Equal("Invalid data.", response.Message);
            Assert.False(response.Success);
        }


        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var dto = new ShippingAddressDto
            {
                Id = 1,
                RecipientName = "Ali",
                City = "Toshkent",
                Country = CountryMethod.Uzbekistan
            };

            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

            var result = await _controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<ShippingAddressDto>>(ok.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenFailed()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            var result = await _controller.Delete(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFound.Value);
            Assert.Equal("ID 1 bilan manzil topilmadi", response.Message);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenSuccess()
        {
            var dto = new ShippingAddressUpdateDto
            {
                RecipientName = "Ali",
                City = "Toshkent",
                Region = "Yunusobod",
                AddressLine = "Ko‘cha 1",
                PostalCode = "100011",
                PhoneNumber = "998901234567",
                Country = CountryMethod.Uzbekistan
            };

            _mockService.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(true);

            var result = await _controller.Update(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(ok.Value);
            Assert.Equal("Manzil muvaffaqiyatli yangilandi", response.Message);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenNotExists()
        {
            var dto = new ShippingAddressUpdateDto
            {
                RecipientName = "Ali",
                City = "Toshkent",
                Region = "Yunusobod",
                AddressLine = "Ko‘cha 1",
                PostalCode = "100011",
                PhoneNumber = "998901234567",
                Country = CountryMethod.Uzbekistan
            };

            _mockService.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(false);

            var result = await _controller.Update(1, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFound.Value);
            Assert.Equal("ID 1 bilan manzil topilmadi", response.Message);
            Assert.False(response.Success);
        }

    }
}
