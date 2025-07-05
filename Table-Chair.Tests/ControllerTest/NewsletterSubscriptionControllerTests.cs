using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Xunit;

namespace Table_Chair_Tests.Controllers
{
    public class NewsletterSubscriptionControllerTests
    {
        private readonly Mock<INewsletterSubscriptionService> _mockService;
        private readonly NewsletterSubscriptionController _controller;

        public NewsletterSubscriptionControllerTests()
        {
            _mockService = new Mock<INewsletterSubscriptionService>();
            _controller = new NewsletterSubscriptionController(_mockService.Object);
        }

        [Fact]
        public async Task Add_ReturnsCreatedResult()
        {
            var dto = new NewsletterSubscriptionCreateDto { Email = "test@example.com" };

            var result = await _controller.Add(dto);

            var okResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, okResult.StatusCode);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResultWithSubscriptions()
        {
            var expected = new List<NewsletterSubscriptionDto> {
                new NewsletterSubscriptionDto { Id = 1, Email = "user1@example.com" }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(expected);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<NewsletterSubscriptionDto>>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task GetById_ReturnsOkResultWithSubscription()
        {
            var expected = new NewsletterSubscriptionDto { Id = 1, Email = "user@example.com" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(expected);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<NewsletterSubscriptionDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal("user@example.com", response.Data.Email);
        }

        [Fact]
        public async Task Update_ReturnsNoContent()
        {
            var dto = new NewsletterSubscriptionUpdateDto { Id = 1, Email = "new@example.com" };

            var result = await _controller.Update(dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task HardDelete_ReturnsNoContent()
        {
            var result = await _controller.HardDelete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task SoftDelete_ReturnsNoContent()
        {
            var result = await _controller.SoftDelete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Restore_ReturnsNoContent()
        {
            var result = await _controller.Restore(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
