using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos.BlogDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Xunit;

namespace Table_Chair.Tests.ControllerTest
{
    public class BlogControllerTest
    {
        private readonly Mock<IBlogService> _blogServiceMock;
        private readonly Mock<ILogger<BlogController>> _loggerMock;
        private readonly BlogController _blogController;

        public BlogControllerTest()
        {
            _blogServiceMock = new Mock<IBlogService>();
            _loggerMock = new Mock<ILogger<BlogController>>();
            _blogController = new BlogController(_blogServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsBlogs()
        {
            var blogs = new List<BlogDto>
            {
                new BlogDto { Id = 1, Title = "Blog 1", Content = "Content" }
            };
            _blogServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(ApiResponse<IEnumerable<BlogDto>>.SuccessResponse(blogs));

            var result = await _blogController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<BlogDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
        }

        [Fact]
        public async Task GetById_BlogExists_ReturnsBlog()
        {
            var blog = new BlogDto { Id = 1, Title = "Test", Content = "Test" };
            _blogServiceMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(ApiResponse<BlogDto>.SuccessResponse(blog));

            var result = await _blogController.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<BlogDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
        }

        [Fact]
        public async Task GetById_BlogNotFound_ReturnsNotFound()
        {
            _blogServiceMock.Setup(x => x.GetByIdAsync(1)).ThrowsAsync(new NotFoundException("Topilmadi"));

            var result = await _blogController.GetById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<BlogDto>>(notFound.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task Create_ValidBlog_ReturnsCreated()
        {
            var dto = new BlogCreateDto { Title = "Test", Content = "Test" };

            _blogServiceMock.Setup(x => x.CreateAsync(dto))
                .ReturnsAsync(ApiResponse<string>.SuccessResponse("Blog created successfully"));

            var result = await _blogController.Create(dto);

            var created = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, created.StatusCode);
            var response = Assert.IsType<ApiResponse<string>>(created.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Update_BlogExists_ReturnsOk()
        {
            var dto = new BlogUpdateDto { Title = "Updated", Content = "Updated" };

            _blogServiceMock.Setup(x => x.UpdateAsync(1, dto))
                .ReturnsAsync(ApiResponse<string>.SuccessResponse("Blog updated successfully"));

            var result = await _blogController.Update(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Update_BlogNotFound_ReturnsNotFound()
        {
            var dto = new BlogUpdateDto { Title = "Updated", Content = "Updated" };

            _blogServiceMock.Setup(x => x.UpdateAsync(1, dto)).ThrowsAsync(new NotFoundException("Topilmadi"));

            var result = await _blogController.Update(1, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFound.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task Delete_BlogExists_ReturnsOk()
        {
            _blogServiceMock.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(ApiResponse<string>.SuccessResponse("Blog deleted successfully"));

            var result = await _blogController.Delete(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Delete_BlogNotFound_ReturnsNotFound()
        {
            _blogServiceMock.Setup(x => x.DeleteAsync(1)).ThrowsAsync(new NotFoundException("Topilmadi"));

            var result = await _blogController.Delete(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFound.Value);
            Assert.False(response.Success);
        }
    }
}