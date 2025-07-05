using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.EmailDtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Xunit;

namespace Table_Chair.Tests.ControllerTest
{
    public class AuthControllerTest
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _authController;

        public AuthControllerTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _authController = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task RegisterUser_ReturnsSuccess()
        {
            var dto = new UserRegisterDto { Email = "test@example.com", Password = "12345678" };
            var userResponse = new UserResponseDto { Username = "testuser", Email = "test@example.com" };

           
            _authServiceMock.Setup(x => x.RegisterAsync(dto)).ReturnsAsync(userResponse);

            var result = await _authController.RegisterUser(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task VerifyEmail_CorrectCode_ReturnsSuccess()
        {
            var dto = new VerifyEmailDto { Email = "test@example.com", Code = "123456" };
            _authServiceMock.Setup(x => x.VerifyEmailAsync(dto.Email, dto.Code)).ReturnsAsync(true);

            var result = await _authController.VerifyEmail(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task VerifyEmail_WrongCode_ReturnsBadRequest()
        {
            var dto = new VerifyEmailDto { Email = "test@example.com", Code = "000000" };
            _authServiceMock.Setup(x => x.VerifyEmailAsync(dto.Email, dto.Code)).ReturnsAsync(false);

            var result = await _authController.VerifyEmail(dto);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task LoginAsync_ReturnsToken()
        {
            var dto = new UserLoginDto { Login = "test@example.com", Password = "12345678" };
            var authResponse = new AuthResponseDto { AccessToken = "token" };

            _authServiceMock.Setup(x => x.LoginAsync(dto)).ReturnsAsync(authResponse);

            var result = await _authController.LoginAsync(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<AuthResponseDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal("token", response.Data.AccessToken);
        }

        [Fact]
        public async Task ForgotPassword_ReturnsSuccess()
        {
            var dto = new ForgotPasswordRequest { Email = "test@example.com" };
            _authServiceMock.Setup(x => x.ForgotPasswordAsync(dto.Email)).ReturnsAsync(true);

            var result = await _authController.ForgotPassword(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }
    }
}

