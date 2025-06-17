using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Xunit;

public class UsersControllerTests
{
// Add the necessary using directive for Moq to resolve the 'Mock<>' type.  

    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<UsersController>> _mockLogger;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_mockUserService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task EmailExists_ReturnsOkResult()
    {
        _mockUserService.Setup(s => s.EmailExistsAsync("test@example.com")).ReturnsAsync(true);

        var result = await _controller.EmailExists("test@example.com");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True(okResult.Value is bool value && value);
    }

    [Fact]
    public async Task GetProfile_ExistingUser_ReturnsOk()
    {
        var profileDto = new UserProfileDto { Id = 1, Username = "test" };
        var userResponseDto = new UserResponseDto
        {
            Id = profileDto.Id,
            Username = profileDto.Username,
            FirstName = profileDto.FirstName,
            LastName = profileDto.LastName,
            PhoneNumber = profileDto.PhoneNumber,
            Email = profileDto.Email,
            AvatarUrl = profileDto.ProfileImageUrl,
            CreatedAt = profileDto.CreatedAt,
            UpdatedAt = profileDto.UpdatedAt
        };

        _mockUserService.Setup(s => s.GetUserProfileAsync(1)).ReturnsAsync(userResponseDto);

        var result = await _controller.GetProfile(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(profileDto, okResult.Value);
    }

    [Fact]
    public async Task GetProfile_NotFound_ReturnsNotFound()
    {
        _mockUserService.Setup(s => s.GetUserProfileAsync(99)).ThrowsAsync(new System.Exception("Not found"));

        var result = await _controller.GetProfile(99);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Not found", notFound.Value);
    }

    [Fact]
    public async Task UpdateProfile_Success_ReturnsOk()
    {
        var dto = new UserUpdateDto { FirstName = "John" };
        _mockUserService.Setup(s => s.UpdateProfileAsync(1, dto)).ReturnsAsync(true);

        var result = await _controller.UpdateProfile(1, dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Profil yangilandi", ok.Value);
    }

    [Fact]
    public async Task UpdateProfile_NotFound_ReturnsNotFound()
    {
        var dto = new UserUpdateDto { FirstName = "John" };
        _mockUserService.Setup(s => s.UpdateProfileAsync(999, dto)).ReturnsAsync(false);

        var result = await _controller.UpdateProfile(999, dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Foydalanuvchi topilmadi", notFound.Value);
    }

    [Fact]
    public async Task VerifyEmail_Success_ReturnsOk()
    {
        _mockUserService.Setup(s => s.VerifyEmailAsync(1)).ReturnsAsync(true);

        var result = await _controller.VerifyEmail(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Email tasdiqlandi", ok.Value);
    }

    [Fact]
    public async Task VerifyEmail_Failure_ReturnsBadRequest()
    {
        _mockUserService.Setup(s => s.VerifyEmailAsync(2)).ReturnsAsync(false);

        var result = await _controller.VerifyEmail(2);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Email tasdiqlanmadi", badRequest.Value);
    }
    [Fact]
    public async Task GetByUsername_ExistingUser_ReturnsOk()
    {
        var userDto = new UserResponseDto { Id = 2, Username = "user1" };
        _mockUserService.Setup(s => s.GetByUsernameAsync("user1")).ReturnsAsync(userDto);

        var result = await _controller.GetByUsername("user1");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userDto, ok.Value);
    }

    [Fact]
    public async Task GetByUsername_NotFound_ReturnsNotFound()
    {
        _mockUserService.Setup(s => s.GetByUsernameAsync("notfound"))
                        .ThrowsAsync(new Exception("User not found"));

        var result = await _controller.GetByUsername("notfound");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found", notFound.Value);
    }
    [Fact]
    public async Task DeleteProfile_Success_ReturnsOk()
    {
        _mockUserService.Setup(s => s.DeleteProfileAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteProfile(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Profil to'liq o'chirildi", ok.Value);
    }

    [Fact]
    public async Task DeleteProfile_NotFound_ReturnsNotFound()
    {
        _mockUserService.Setup(s => s.DeleteProfileAsync(999)).ReturnsAsync(false);

        var result = await _controller.DeleteProfile(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Foydalanuvchi topilmadi", notFound.Value);
    }
    [Fact]
    public async Task SoftDeleteProfile_Success_ReturnsOk()
    {
        _mockUserService.Setup(s => s.DeleteOwnProfileAsync(1)).ReturnsAsync(true);

        var result = await _controller.SoftDeleteProfile(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Profil o'chirildi", ok.Value);
    }

    [Fact]
    public async Task SoftDeleteProfile_AlreadyDeleted_ReturnsNotFound()
    {
        _mockUserService.Setup(s => s.DeleteOwnProfileAsync(2)).ReturnsAsync(false);

        var result = await _controller.SoftDeleteProfile(2);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Foydalanuvchi topilmadi yoki allaqachon o'chirilgan", notFound.Value);
    }
    [Fact]
    public async Task GetByEmail_ExistingUser_ReturnsOk()
    {
        var userDto = new UserResponseDto { Id = 1, Email = "user@example.com" };
        _mockUserService.Setup(s => s.GetByEmailAsync("user@example.com")).ReturnsAsync(userDto);

        var result = await _controller.GetByEmail("user@example.com");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userDto, ok.Value);
    }

    [Fact]
    public async Task GetByEmail_NotFound_ReturnsNotFound()
    {
        _mockUserService.Setup(s => s.GetByEmailAsync("notfound@example.com"))
                        .ThrowsAsync(new Exception("User not found"));

        var result = await _controller.GetByEmail("notfound@example.com");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found", notFound.Value);
    }
    [Fact]
    public async Task GetByPhone_ExistingUser_ReturnsOk()
    {
        var userDto = new UserResponseDto { Id = 1, PhoneNumber = "+998901112233" };
        _mockUserService.Setup(s => s.GetByPhoneAsync("+998901112233")).ReturnsAsync(userDto);

        var result = await _controller.GetByPhone("+998901112233");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userDto, ok.Value);
    }

    [Fact]
    public async Task GetByPhone_NotFound_ReturnsNotFound()
    {
        _mockUserService.Setup(s => s.GetByPhoneAsync("+998900000000"))
                        .ThrowsAsync(new Exception("User not found"));

        var result = await _controller.GetByPhone("+998900000000");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found", notFound.Value);
    }

    [Fact]
    public async Task UsernameExists_Existing_ReturnsTrue()
    {
        _mockUserService.Setup(s => s.UsernameExistsAsync("admin")).ReturnsAsync(true);

        var result = await _controller.UsernameExists("admin");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.True(ok.Value is bool value && value);
    }

    [Fact]
    public async Task UsernameExists_NotExisting_ReturnsFalse()
    {
        _mockUserService.Setup(s => s.UsernameExistsAsync("nope")).ReturnsAsync(false);

        var result = await _controller.UsernameExists("nope");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.False(ok.Value is bool value && value);
    }
    [Fact]
    public async Task PhoneExists_Existing_ReturnsTrue()
    {
        _mockUserService.Setup(s => s.PhoneExistsAsync("+998911223344")).ReturnsAsync(true);

        var result = await _controller.PhoneExists("+998911223344");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.True(ok.Value is bool value && value);
    }

    [Fact]
    public async Task PhoneExists_NotExisting_ReturnsFalse()
    {
        _mockUserService.Setup(s => s.PhoneExistsAsync("+998900000000")).ReturnsAsync(false);

        var result = await _controller.PhoneExists("+998900000000");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.False(ok.Value is bool value && value);
    }

}

