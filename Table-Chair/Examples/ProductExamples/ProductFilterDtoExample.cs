using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.AdditionDtos;

public class ProductFilterDtoExample : IExamplesProvider<ProductFilterDto>
{
    public ProductFilterDto GetExamples()
    {
        return new ProductFilterDto
        {
            CategoryId = 2,
            MinPrice = 50m,
            MaxPrice = 500m,
            SearchQuery = "stol",
            SortBy = "price",
            IsAscending = true,
            MinStockQuantity = 5
        };
    }
}

