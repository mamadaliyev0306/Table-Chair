using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos;

public class UpdateProductDtoExample : IExamplesProvider<UpdateProductDto>
{
    public UpdateProductDto GetExamples()
    {
        return new UpdateProductDto
        {
            Id = 1,
            Name = "Yangi dizayndagi stul",
            Description = "Qulay o‘rindiqli, qora rangli stul",
            Price = 89.99m,
            ImageUrl = "https://cdn.example.com/images/chair456.jpg",
            CategoryId = 2,
            StockQuantity = 15,
            IsDeleted = false
        };
    }
}

