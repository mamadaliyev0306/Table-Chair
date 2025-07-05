using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Xunit;

namespace Table_Chair.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UsersController>> _mockLogger;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UsersController>>();
            _controller = new UsersController(_mockUserService.Object);
        }

        #region EmailExists Tests

        [Fact]
        public async Task EmailExists_WhenEmailExists_ReturnsTrue()
        {
            // Arrange
            const string testEmail = "test@example.com";
            _mockUserService.Setup(x => x.EmailExistsAsync(testEmail))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.EmailExists(testEmail);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task EmailExists_WhenEmailNotExists_ReturnsFalse()
        {
            // Arrange
            const string testEmail = "notfound@example.com";
            _mockUserService.Setup(x => x.EmailExistsAsync(testEmail))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.EmailExists(testEmail);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
            Assert.False(response.Data);
        }

        #endregion

        #region GetProfile Tests

        [Fact]
        public async Task GetProfile_WhenUserExists_ReturnsUserProfile()
        {
            // Arrange
            var expectedProfile = new UserProfileDto
            {
                Id = 1,
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PhoneNumber = "+998901234567",
                AvatarUrl = "https://example.com/profile.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow,
                Bio="Salom"
            };

            _mockUserService.Setup(x => x.GetUserProfileAsync(1))
                .ReturnsAsync(expectedProfile);

            // Act
            var result = await _controller.GetProfile(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<UserProfileDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(expectedProfile.Id, response.Data.Id);
            Assert.Equal(expectedProfile.Username, response.Data.Username);
        }

        [Fact]
        public async Task GetProfile_WhenUserNotExists_ReturnsNotFound()
        {
            // Arrange
            _mockUserService.Setup(x => x.GetUserProfileAsync(999))
                .ThrowsAsync(new Exception("User not found"));

            // Act
            var result = await _controller.GetProfile(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
            Assert.Equal("User not found", response.Message);
        }

        #endregion

        #region UpdateProfile Tests

        [Fact]
        public async Task UpdateProfile_WhenValidData_ReturnsSuccess()
        {
            // Arrange
            var updateDto = new UserUpdateDto
            {
                FirstName = "Updated",
                LastName = "User",
                PhoneNumber = "+998901112233"
            };

            _mockUserService.Setup(x => x.UpdateProfileAsync(1, updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProfile(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Profil muvaffaqiyatli yangilandi", response.Message);
        }

        [Fact]
        public async Task UpdateProfile_WhenUserNotExists_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new UserUpdateDto();
            _mockUserService.Setup(x => x.UpdateProfileAsync(999, updateDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateProfile(999, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
            Assert.Equal("Foydalanuvchi topilmadi", response.Message);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task SoftDeleteProfile_WhenValidUser_ReturnsSuccess()
        {
            // Arrange
            _mockUserService.Setup(x => x.DeleteOwnProfileAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.SoftDeleteProfile(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Profil soft delete qilindi", response.Message);
        }

        [Fact]
        public async Task DeleteProfile_WhenAdminRequest_ReturnsSuccess()
        {
            // Arrange
            _mockUserService.Setup(x => x.DeleteProfileAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteProfile(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Profil to‘liq o‘chirildi", response.Message.Normalize());
        }

        #endregion

        #region Verification Tests

        [Fact]
        public async Task VerifyEmail_WhenValidToken_ReturnsSuccess()
        {
            // Arrange
            _mockUserService.Setup(x => x.VerifyEmailAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.VerifyEmail(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Email muvaffaqiyatli tasdiqlandi", response.Message);
        }

        [Fact]
        public async Task VerifyEmail_WhenInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            _mockUserService.Setup(x => x.VerifyEmailAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.VerifyEmail(999);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Email tasdiqlanmadi", response.Message);
        }

        #endregion

        #region GetBy Tests

        [Fact]
        public async Task GetByEmail_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var expectedUser = new UserResponseDto
            {
                Id = 1,
                Email = "test@example.com"
            };

            _mockUserService.Setup(x => x.GetByEmailAsync("test@example.com"))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetByEmail("test@example.com");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<UserResponseDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(expectedUser.Id, response.Data.Id);
        }

        [Fact]
        public async Task GetByUsername_WhenUserNotExists_ReturnsNotFound()
        {
            // Arrange
            _mockUserService.Setup(x => x.GetByUsernameAsync("unknown"))
               .ThrowsAsync(new NotFoundException("User not found"));

            // Act
            var result = await _controller.GetByUsername("unknown");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<UserResponseDto>>(notFoundResult.Value);
            Assert.Equal("User not found", response.Message);
        }

        #endregion
    }
}
