using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos.AboutInfoDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Tests.ControllerTest
{
    public class AboutInfoControllerTest
    {
        private readonly Mock<IAboutInfoService> _aboutInfoServiceMock;
        private readonly Mock<ILogger<AboutInfoController>> _loggerMock;
        private readonly AboutInfoController _controller;

        public AboutInfoControllerTest()
        {
            _aboutInfoServiceMock = new Mock<IAboutInfoService>();
            _loggerMock = new Mock<ILogger<AboutInfoController>>();
            _controller = new AboutInfoController(_aboutInfoServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfAboutInfo()
        {
            // Arrange  
            var list = new List<AboutInfoDto>
               {
                   new AboutInfoDto {Id=1,Content="Salom1",ImageUrl ="/images/rasm1",CreatedAt=DateTime.UtcNow,DeletedAt=DateTime.UtcNow,UpdateAt=DateTime.UtcNow},
                   new AboutInfoDto {Id=2,Content="Salom2",ImageUrl ="/images/rasm2",CreatedAt=DateTime.UtcNow,DeletedAt=DateTime.UtcNow,UpdateAt=DateTime.UtcNow}
               };

            _aboutInfoServiceMock.Setup(a => a.GetAllAsync())
                .ReturnsAsync(ApiResponse<IEnumerable<AboutInfoDto>>.SuccessResponse(list));

            // Act  
            var result = await _controller.GetAll();

            // Assert  
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<AboutInfoDto>>>(okResult.Value);
            Assert.NotNull(response.Data); 
            Assert.Equal(2, response.Data!.Count());  
        }
        [Fact]
        public async Task GetById_ReturnsAboutInfo_WhenFound()
        {
            //Arrange
            var dto = new AboutInfoDto { Id = 1, Content = "Salom1", ImageUrl = "/images/rasm1", CreatedAt = DateTime.UtcNow, DeletedAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow };
             _aboutInfoServiceMock.Setup(x=>x.GetByIdAsync(1))
                .ReturnsAsync(ApiResponse<AboutInfoDto>.SuccessResponse(dto));
            //Act 
            var result = await _controller.GetById(1);
            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<AboutInfoDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal("Salom1",response.Data.Content);
        }
        [Fact]
        public async Task Create_ValidDto_ReturnsSuccess()
        {
            // Arrange  
            var dto = new AboutInfoCreateDto { Content = "About 1", ImageUrl = "/images/rasm1" };
            _aboutInfoServiceMock.Setup(a => a.CreateAsync(dto))
                .ReturnsAsync(ApiResponse<string>.SuccessResponse("AboutInfo successfully created!"));

            // Act  
            var result = await _controller.Create(dto);

            // Assert  
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("AboutInfo successfully created!", response.Message);
        }
        [Fact]
        public async Task Update_ValidDto_ReturnsSuccess()
        {
            // Arrange
            var dto = new AboutInfoUpdateDto { Content = "About 1", ImageUrl = "/images/rasm1" };
            _aboutInfoServiceMock.Setup(a => a.UpdateAsync(1, dto))
                .ReturnsAsync(ApiResponse<string>.SuccessResponse("AboutInfo successfully updated!"));

            //Act
            var result = await _controller.Update(1, dto);

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult?.Value);
            Assert.Equal("AboutInfo successfully updated!", response.Message);
        }
        [Fact]
        public async Task Delete_ValidId_ReturnsSuccess()
        {
            //Arrange
            _aboutInfoServiceMock.Setup( x => x.DeleteAsync(1))
                .ReturnsAsync(ApiResponse<string>.SuccessResponse(string.Empty, "AboutInfo successfully deleted!"));
            //Act 
            var result = await _controller.Delete(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("AboutInfo successfully deleted!", response.Message);
        }
    }
}
