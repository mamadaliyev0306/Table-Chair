using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Entity.Enums;

namespace Table_Chair.Examples.CategoryExample
{
    public class CategoryCreateDtoExample : IExamplesProvider<CategoryCreateDto>
    {
        public CategoryCreateDto GetExamples()
        {
            return new CategoryCreateDto()
            {
                Name = "name",
                Type=CategoryType.Product
            };
        }
    }
}
