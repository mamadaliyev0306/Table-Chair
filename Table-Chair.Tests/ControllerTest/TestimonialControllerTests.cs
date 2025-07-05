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
    public class TestimonialControllerTests
    {
        private readonly Mock<ITestimonialService> _serviceMock;
        private readonly TestimonialController _controller;

        public TestimonialControllerTests()
        {
            _serviceMock = new Mock<ITestimonialService>();
            _controller = new TestimonialController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfTestimonials()
        {
            // Arrange
            var testimonials = new List<TestimonialDto> {
                new TestimonialDto { Id = 1, AuthorName = "Ali", Content = "Zo'r xizmat" }
            };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(testimonials);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<TestimonialDto>>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task GetById_ShouldReturnTestimonial()
        {
            // Arrange
            var testimonial = new TestimonialDto { Id = 1,AuthorName = "Ali", Content = "Ajoyib!" };
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(testimonial);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<TestimonialDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Ali", response.Data!.AuthorName);
        }

        [Fact]
        public async Task Create_ShouldReturnSuccessMessage()
        {
            // Arrange
            var dto = new CreateTestimonialDto { AuthorName = "Ali", Content = "Zo‘r" };

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            _serviceMock.Verify(s => s.CreateAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnSuccessMessage()
        {
            // Arrange
            var dto = new UpdateTestimonialDto { Id = 1, AuthorName = "Ali", Content = "Yangilangan fikr" };

            // Act
            var result = await _controller.Update(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            _serviceMock.Verify(s => s.UpdateAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnSuccessMessage()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            _serviceMock.Verify(s => s.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task SoftDelete_ShouldReturnSuccessMessage()
        {
            var id = 1;

            var result = await _controller.SoftDelete(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            _serviceMock.Verify(s => s.SoftDeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task Restore_ShouldReturnSuccessMessage()
        {
            var id = 1;

            var result = await _controller.Restore(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            _serviceMock.Verify(s => s.RestoreAsync(id), Times.Once);
        }
    }
}
