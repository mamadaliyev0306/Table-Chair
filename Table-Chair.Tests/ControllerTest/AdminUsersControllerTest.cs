using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;

namespace Table_Chair.Tests.ControllerTest
{
    public  class AdminUsersControllerTest
    {
        private readonly Mock<IAdminUserService> _adminUserServiceMock;
        private readonly Mock<ILogger<AdminUsersController>> _loggerMock;
        private readonly AdminUsersController _adminUsersControllerMock;
        public AdminUsersControllerTest()
        {
            _adminUserServiceMock= new Mock<IAdminUserService>();
            _loggerMock = new Mock<ILogger<AdminUsersController>>();
            _adminUsersControllerMock = new AdminUsersController(_adminUserServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetById_ReturnsUser()
        {
            //Arrange
            var user = new AdminUserResponseDto
            {
                Id = 1,
                FirstName = "Ali",
                LastName = "Aliyev",
                PhoneNumber = "+998940031432",
                Email = "ali@gmail.com",
                AvatarUrl = "/images/users",
                Bio = "Salom men dasturchiman",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                EmailVerified = false,
                IsActive = false,
                Role = Role.Customer,
                Username = "ali03",
                IsDelete = false,
                LastLoginDate = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };
            _adminUserServiceMock.Setup(a => a.GetUserByIdAsync(1))
                .ReturnsAsync(user);

            //Act 
            var result = await _adminUsersControllerMock.GetById(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<UserResponseDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
        }
        [Fact]
        public async Task Update_UserExists_ReturnsSuccess()
        {
            //Arrange 
            var user = new AdminUserUpdateDto
            {
                FirstName = "Ali",
                LastName ="Aliyev",
                Email ="aliyev@gmail.com",
                IsActive=false,
                EmailVerified=false,
                PhoneNumber = "1234567890"
            };

            _adminUserServiceMock.Setup(x => x.UpdateUserAsync(1, user))
                .ReturnsAsync(true);
            //Act 
            var result= await _adminUsersControllerMock.Update(1, user);
            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }
        [Fact]
        public async Task Update_UserNotFound_ReturnsNotFound()
        {
            //Arrange 
            var user = new AdminUserUpdateDto
            {
                FirstName = "Ali",
                LastName = "Aliyev",
                Email = "aliyev@gmail.com",
                IsActive = false,
                EmailVerified = false,
                PhoneNumber = "1234567890"
            };

            _adminUserServiceMock.Setup(x => x.UpdateUserAsync(1, user))
                .ReturnsAsync(false);
            //Act 
            var result = await _adminUsersControllerMock.Update(1, user);
            //Assert
            var notfount = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notfount.Value);
            Assert.False(response.Success);
        }
        [Fact]
        public async Task ChangeRole_UserExists_ReturnsSuccess()
        {
            //Arrange
            _adminUserServiceMock.Setup(a=>a.ChangeUserRoleAsync(1,Role.Admin)).ReturnsAsync(true);
            //Act 
            var result = await _adminUsersControllerMock.ChangeRole(1,Role.Admin);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }
        [Fact]
        public async Task GetDeletedUsers_ReturnsList()
        {
            //Arrange 
            var userlist = new List<AdminUserResponseDto>()
            { new AdminUserResponseDto()
            {
                Id = 1,
                FirstName = "Ali",
                LastName = "Aliyev",
                PhoneNumber = "+998940031432",
                Email = "ali@gmail.com",
                AvatarUrl = "/images/users",
                Bio = "Salom men dasturchiman",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                EmailVerified = false,
                IsActive = false,
                Role = Role.Customer,
                Username = "ali03",
                IsDelete = false,
                LastLoginDate = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            }
            };
            _adminUserServiceMock.Setup(a=>a.GetDeletedUsersAsync())
                .ReturnsAsync(userlist);
            //Act 
            var result = await _adminUsersControllerMock.GetDeletedUsers();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<AdminUserResponseDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Single(response.Data);
        }
        [Fact]
        public async Task GetUserCountStats_ReturnsStats()
        {
            // Arrange
            var stats = new UserCountStatsDto
            {
                TotalUsers = 5,
                ActiveUsers = 2,
                DeletedUsers = 1,
                InactiveUsers = 2,
                UsersByRole = new Dictionary<Role, int>
        {
            { Role.Admin, 1 },
            { Role.Customer, 4 }
        }
            };

            _adminUserServiceMock.Setup(x => x.GetUserCountStatsAsync())
                .ReturnsAsync(stats);

            // Act
            var result = await _adminUsersControllerMock.GetUserCountStats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<UserCountStatsDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(5, response.Data.TotalUsers);
            Assert.Equal(2, response.Data.ActiveUsers);
            Assert.Equal(1, response.Data.DeletedUsers);
            Assert.Equal(2, response.Data.InactiveUsers);
            Assert.Equal(2, response.Data.UsersByRole.Count);
            Assert.Equal(1, response.Data.UsersByRole[Role.Admin]);
            Assert.Equal(4, response.Data.UsersByRole[Role.Customer]);
        }

    }
}
