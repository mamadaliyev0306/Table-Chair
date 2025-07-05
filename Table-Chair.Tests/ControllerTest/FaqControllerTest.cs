using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Table_Chair.Controllers;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;
using Xunit;

namespace Table_Chair_Tests.Controllers
{
    public class FaqControllerTests
    {
        private readonly Mock<IFaqService> _faqServiceMock;
        private readonly FaqController _controller;

        public FaqControllerTests()
        {
            _faqServiceMock = new Mock<IFaqService>();
            _controller = new FaqController(_faqServiceMock.Object);
        }

        [Fact]
        public async Task Create_ReturnsOk()
        {
            var dto = new FaqCreateDto { CategoryId = 1, Title = "Test", Answer = "A." };

            var result = await _controller.Create(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.NotNull(response); 
            Assert.True(response.Success);
        }

        [Fact]
        public async Task GetById_ReturnsFaqDto()
        {
            var faq = new FaqDto { Id = 1, CategoryId=1,CategoryName="Savollar",Title="Savol", Answer = "A.",CreatedAt=DateTime.UtcNow };
            _faqServiceMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(faq);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<FaqDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(faq.Id, response.Data.Id);
        }

        [Fact]
        public async Task Delete_ReturnsOk()
        {
            var result = await _controller.Delete(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.NotNull(response); 
            Assert.True(response.Success);
        }

        [Fact]
        public async Task GetAll_ReturnsList()
        {
            Expression<Func<Faq, bool>>? filter = null;
            Func<IQueryable<Faq>, IOrderedQueryable<Faq>>? orderBy = null;
            int? page = null;
            int? pageSize = null;

            _faqServiceMock.Setup(x => x.GetAllAsync(filter, orderBy, page, pageSize))
                .ReturnsAsync(new List<FaqDto>
                {
                  new FaqDto { Id = 1 },
                  new FaqDto { Id = 2 }
                });

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<FaqDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.Count);
        }

        [Fact]
        public async Task Search_ReturnsResults()
        {
            _faqServiceMock.Setup(x => x.SearchAsync("test"))
                .ReturnsAsync(new List<FaqDto> { new FaqDto { Id = 3 } });

            var result = await _controller.Search("test");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<FaqDto>>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Single(response.Data);
        }

        [Fact]
        public async Task Exists_ReturnsSuccess()
        {
            _faqServiceMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Faq, bool>>>()))
                .ReturnsAsync(true);

            var result = await _controller.Exists("Question", "Test");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
            Assert.True(response.Data);
        }
    }
}

