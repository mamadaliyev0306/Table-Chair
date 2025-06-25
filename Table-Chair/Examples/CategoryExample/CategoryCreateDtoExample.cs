using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.CreateDtos;

namespace Table_Chair.Examples.CategoryExample
{
    public class CategoryCreateDtoExample : IExamplesProvider<CategoryCreateDto>
    {
        public CategoryCreateDto GetExamples()
        {
            return new CategoryCreateDto()
            {
                Name = "name",
                IsActive = true
            };
        }
    }
}
