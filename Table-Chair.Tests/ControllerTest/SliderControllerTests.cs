using Microsoft.AspNetCore.Mvc;
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
    public class SliderControllerTests
    {
        private readonly Mock<ISliderService> _sliderServiceMock;
        private readonly SliderController _controller;

        public SliderControllerTests()
        {
            _sliderServiceMock = new Mock<ISliderService>();
            _controller = new SliderController(_sliderServiceMock.Object);
        }

        [Fact]
        public async Task GetSliderListAsync_ReturnsSliderList()
        {
            // Arrange
            var sliders = new List<SliderDto>
            {
                new SliderDto { Id = 1, Title = "Test Slider", IsActive = true }
            };

            _sliderServiceMock.Setup(s => s.GetSliderListAsync()).ReturnsAsync(sliders);

            // Act
            var result = await _controller.GetSliderListAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<SliderDto>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Single(response.Data!);
        }

        [Fact]
        public async Task GetSliderByIdAsync_ReturnsSlider()
        {
            var slider = new SliderDto { Id = 1, Title = "Slider 1", IsActive = true };
            _sliderServiceMock.Setup(s => s.GetSliderByIdAsync(1)).ReturnsAsync(slider);

            var result = await _controller.GetSliderByIdAsync(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<SliderDto>>(okResult.Value);
            Assert.Equal(1, response.Data!.Id);
        }

        [Fact]
        public async Task AddSliderAsync_ReturnsSuccessMessage()
        {
            var dto = new CreateSliderDto
            {
                ImageUrl = "https://example.com/image.jpg",
                Title = "New Slider",
                IsActive = true
            };

            var result = await _controller.AddSliderAsync(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            _sliderServiceMock.Verify(s => s.AddSliderAsync(dto), Times.Once);
        }

        [Fact]
        public async Task UpdateSliderAsync_ReturnsSuccessMessage()
        {
            var dto = new SliderUpdateDto
            {
                Id = 1,
                ImageUrl = "https://example.com/image.jpg",
                Title = "Updated Slider",
                IsActive = true
            };

            var result = await _controller.UpdateSliderAsync(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            _sliderServiceMock.Verify(s => s.UpdateSliderAsync(dto), Times.Once);
        }

        [Fact]
        public async Task DeleteSliderAsync_ReturnsSuccessMessage()
        {
            int id = 1;

            var result = await _controller.DeleteSliderAsync(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            _sliderServiceMock.Verify(s => s.DeleteSliderAsync(id), Times.Once);
        }
    }
}
