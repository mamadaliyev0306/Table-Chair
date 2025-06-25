using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.CreateDtos;

public class CreateProductDtoExample : IExamplesProvider<CreateProductDto>
{
    public CreateProductDto GetExamples()
    {
        return new CreateProductDto
        {
            Name = "Ergonomik stol",
            Description = "Sifatli yog'ochdan tayyorlangan zamonaviy stol",
            Price = 499.99m,
            ImageUrl = "https://cdn.example.com/images/table123.jpg",
            StockQuantity = 20,
            CategoryId = 3,
            DiscountPercent = 10
        };
    }
}

