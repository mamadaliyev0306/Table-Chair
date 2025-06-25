using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.BlogDtos;

namespace Table_Chair.Examples.BlogExample
{
    public class BlogUpdateDtoExample : IExamplesProvider<BlogUpdateDto>
    {
        public BlogUpdateDto GetExamples()
        {
            return new BlogUpdateDto()
            {
                CategoryId = 1,
                Title = "Title",
                Content ="Content",
                ImageUrl = "https://example.com/image.png"
            };
        }
    }
}
