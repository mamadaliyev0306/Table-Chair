using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Tests.ControllerTest
{
    public class TokenControllerTests
    {
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly TokenController _controller;

        public TokenControllerTests()
        {
            _tokenServiceMock = new Mock<ITokenService>();
            _controller = new TokenController(_tokenServiceMock.Object);
        }

        [Fact]
        public void ValidateEmailVerificationToken_ValidToken_ReturnsOk()
        {
            // Arrange
            string token = "valid_token";
            int expectedUserId = 1;

            _tokenServiceMock.Setup(x => x.ValidateEmailVerificationToken(token))
                             .Returns(expectedUserId);

            // Act
            var result = _controller.ValidateEmailVerificationToken(token) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = Assert.IsType<ApiResponse<TokenValidationResultDto>>(result.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(expectedUserId, response.Data.UserId);
            Assert.True(response.Success);
            Assert.Equal("Token yaroqli", response.Message);
        }


        [Fact]
        public void ValidateEmailVerificationToken_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            string token = "invalid_token";
            _tokenServiceMock.Setup(x => x.ValidateEmailVerificationToken(token))
                             .Returns((int?)null);

            // Act
            var result = _controller.ValidateEmailVerificationToken(token) as UnauthorizedObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = Assert.IsType<ApiResponse<string>>(result.Value);
            Assert.False(response.Success);
            Assert.Equal("Token noto‘g‘ri yoki muddati o‘tgan.", response.Message);
        }

        [Fact]
        public void ValidatePasswordResetToken_ValidToken_ReturnsOk()
        {
            // Arrange
            string token = "valid_reset_token";
            int expectedUserId = 2;

            _tokenServiceMock.Setup(x => x.ValidatePasswordResetToken(token))
                             .Returns(expectedUserId);

            // Act
            var result = _controller.ValidatePasswordResetToken(token) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = Assert.IsType<ApiResponse<TokenValidationResultDto>>(result.Value);
            Assert.NotNull(response.Data); 
            Assert.Equal(expectedUserId, response.Data.UserId);
            Assert.True(response.Success);
            Assert.Equal("Token yaroqli", response.Message);
        }
            [Fact]
        public void ValidatePasswordResetToken_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            string token = "expired_or_invalid";
            _tokenServiceMock.Setup(x => x.ValidatePasswordResetToken(token))
                             .Returns((int?)null);

            // Act
            var result = _controller.ValidatePasswordResetToken(token) as UnauthorizedObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = Assert.IsType<ApiResponse<string>>(result.Value);
            Assert.False(response.Success);
            Assert.Equal("Token noto‘g‘ri yoki muddati o‘tgan.", response.Message);
        }
    }

}
