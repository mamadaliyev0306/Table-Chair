using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos.EmailDtos;
using Table_Chair_Application.Emails;
using Table_Chair_Application.Responses;
using Xunit;

namespace Table_Chair.Tests.ControllerTests
{
    public class EmailControllerTests
    {
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ILogger<EmailController>> _mockLogger;
        private readonly EmailController _controller;

        public EmailControllerTests()
        {
            _mockEmailService = new Mock<IEmailService>();
            _mockLogger = new Mock<ILogger<EmailController>>();
            _controller = new EmailController(_mockEmailService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SendPasswordReset_ReturnsOk()
        {
            var request = new SendPasswordResetRequest
            {
                Email = "test@example.com",
                Name = "Test User",
                Token = "token123"
            };

            _mockEmailService.Setup(s => s.SendPasswordResetAsync(request.Email, request.Name, request.Token))
                .Returns(Task.CompletedTask);

            var result = await _controller.SendPasswordReset(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task SendOrderConfirmation_ReturnsOk()
        {
            var request = new SendOrderConfirmationRequest
            {
                Email = "order@example.com",
                Name = "Order Name",
                OrderId = "ORD123"
            };

            _mockEmailService.Setup(s => s.SendOrderConfirmationAsync(request.Email, request.Name, request.OrderId))
                .Returns(Task.CompletedTask);

            var result = await _controller.SendOrderConfirmation(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task SendPasswordChangedNotification_ReturnsOk()
        {
            var request = new SendPasswordChangedNotificationRequest
            {
                Email = "user@example.com",
                Name = "User Name"
            };

            _mockEmailService.Setup(s => s.SendPasswordChangedNotificationAsync(request.Email, request.Name))
                .Returns(Task.CompletedTask);

            var result = await _controller.SendPasswordChangedNotification(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task SendNewUserNotificationToAdmin_ReturnsOk()
        {
            var request = new SendNewUserNotificationToAdminRequest
            {
                AdminEmail = "admin@example.com",
                NewUserEmail = "newuser@example.com"
            };

            _mockEmailService.Setup(s => s.SendNewUserNotificationToAdminAsync(request.AdminEmail, request.NewUserEmail))
                .Returns(Task.CompletedTask);

            var result = await _controller.SendNewUserNotificationToAdmin(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }
    }
}

