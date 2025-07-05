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

namespace Table_Chair.Tests.ControllerTests
{
    public class ContactMessageControllerTests
    {
        private readonly Mock<IContactMessageService> _mockService;
        private readonly Mock<ILogger<ContactMessageController>> _mockLogger;
        private readonly ContactMessageController _controller;

        public ContactMessageControllerTests()
        {
            _mockService = new Mock<IContactMessageService>();
            _mockLogger = new Mock<ILogger<ContactMessageController>>();
            _controller = new ContactMessageController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsContactMessages()
        {
            // Arrange
            var messages = new List<ContactMessageDto>
            {
                new ContactMessageDto 
                {   Id = 1,
                    FirstName = "Ali ",
                    LastName="Aliyev",
                    Email = "ali@example.com",
                    Message = "Salom",
                    PhoneNumber="998940031432",
                    IsRead=true,
                    IsResponded=true,
                    SentAt=DateTime.Now
                }
            };
            _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(messages);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<ContactMessageDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Single(response.Data);
        }

        [Fact]
        public async Task GetById_ReturnsSingleMessage()
        {
            // Arrange
            var message = new ContactMessageDto { Id = 1, FirstName = "Ali ", LastName = "Aliyev", Email = "ali@example.com", Message = "Salom" };
            _mockService.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(message);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<ContactMessageDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResponse()
        {
            // Arrange
            var dto = new ContactMessageCreateDto
            {
                FirstName = "Ali ",
                LastName = "Aliyev",
                Email = "ali@example.com",
                Message = "Salom!",
                PhoneNumber = "1234522456",
            };

            _mockService.Setup(x => x.CreateAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var response = Assert.IsType<ApiResponse<string>>(createdResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            _mockService.Setup(x => x.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
