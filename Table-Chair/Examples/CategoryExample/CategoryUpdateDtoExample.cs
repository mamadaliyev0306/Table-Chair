using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos;

namespace Table_Chair.Examples.CategoryExample
{
    public class CategoryUpdateDtoExample : IExamplesProvider<CategoryUpdateDto>
    {
        public CategoryUpdateDto GetExamples()
        {
            return new CategoryUpdateDto()
            {
                Id = 1,
                Name = "Name",
                IsActive = true
            };
        }
    }
}
