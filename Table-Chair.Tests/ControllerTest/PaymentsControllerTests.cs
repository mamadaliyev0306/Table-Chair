using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos.PaymentDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Entity.Enums;

namespace Table_Chair.Tests.ControllerTest
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly Mock<ILogger<PaymentsController>> _mockLogger;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _mockPaymentService = new Mock<IPaymentService>();
            _mockLogger = new Mock<ILogger<PaymentsController>>();
            _controller = new PaymentsController(_mockPaymentService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreatePayment_Returns_Created_Response()
        {
            // Arrange
            var dto = new PaymentCreateDto { OrderId = 1, Amount = 100, PaymentMethod = PaymentMethod.Payme };
            var expected = new PaymentResponseDto { Id = 10, OrderId = 1, Amount = 100, Status = PaymentStatus.Completed };

            _mockPaymentService
                .Setup(s => s.CreatePaymentAsync(dto))
                .ReturnsAsync(expected); // Explicitly cast to nullable type

            // Act
            var result = await _controller.CreatePayment(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<ApiResponse<PaymentResponseDto>>(createdResult.Value);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(expected.Id, response.Data.Id);
        }

        [Fact]
        public async Task GetPaymentById_Returns_Ok_If_Found()
        {
            // Arrange
            var paymentId = 5;
            var expected = new PaymentResponseDto { Id = paymentId, OrderId = 2, Amount = 150 };

            _mockPaymentService
                .Setup(s => s.GetPaymentByIdAsync(paymentId))
                .ReturnsAsync(expected);

            // Act
            var result = await _controller.GetPaymentById(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<PaymentResponseDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(expected.Id, response.Data.Id);
        }

        [Fact]
        public async Task GetPaymentById_Returns_NotFound_If_Null()
        {
            // Arrange
            int paymentId = 9;

            _mockPaymentService
                .Setup(s => s.GetPaymentByIdAsync(paymentId))
                .ReturnsAsync((PaymentResponseDto?)null);

            // Act
            var result = await _controller.GetPaymentById(paymentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<PaymentResponseDto>>(notFoundResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdatePaymentStatus_Returns_Ok()
        {
            // Arrange
            int id = 1;
            var dto = new PaymentUpdateDto { Id = id, Amount = 100, Status = PaymentStatus.Completed };
            var expected = new PaymentResponseDto { Id = id, Amount = 100, Status = PaymentStatus.Completed };

            _mockPaymentService
                .Setup(s => s.UpdatePaymentStatusAsync(id, dto))
                .ReturnsAsync(expected);

            // Act
            var result = await _controller.UpdatePaymentStatus(id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<PaymentResponseDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(expected.Id, response.Data.Id);
        }

        [Fact]
        public async Task RefundPayment_Returns_Ok()
        {
            // Arrange
            int id = 2;
            var refundDto = new RefundRequestDto { TransactionId = "ABC123", Amount = 50, Reason = "User requested" };
            var expected = new PaymentResponseDto { Id = id, Amount = 50, Status = PaymentStatus.Refunded };

            _mockPaymentService
                .Setup(s => s.RefundPaymentAsync(id, refundDto))
                .ReturnsAsync(expected);

            // Act
            var result = await _controller.RefundPayment(id, refundDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<PaymentResponseDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(PaymentStatus.Refunded, response.Data.Status);
        }
    }
}

