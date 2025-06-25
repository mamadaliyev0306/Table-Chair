using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.CreateDtos;

public class SliderCreateDtoExample : IExamplesProvider<CreateSliderDto>
{
    public CreateSliderDto GetExamples()
    {
        return new CreateSliderDto
        {
            ImageUrl = "https://cdn.example.com/images/slider1.jpg",
            Title = "Yozgi chegirmalar!",
            Description = "Bugun boshlang, 50% gacha chegirma!",
            RedirectUrl = "/products/sale",
            IsActive = true
        };
    }
}
