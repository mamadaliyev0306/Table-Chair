using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.BlogDtos;

namespace Table_Chair.Examples.BlogExample
{
    public class BlogCreateDtoExample : IExamplesProvider<BlogCreateDto>
    {
        public BlogCreateDto GetExamples()
        {
            return new BlogCreateDto()
            {
                CategoryId = 1,
                Title = "Title",
                Content = "Content",
                ImageUrl = "https://example.com/image.png"
            };
        }
    }
}
