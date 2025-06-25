using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos;

public class SliderUpdateDtoExample : IExamplesProvider<SliderUpdateDto>
{
    public SliderUpdateDto GetExamples()
    {
        return new SliderUpdateDto
        {
            Id = 1,
            ImageUrl = "https://cdn.example.com/images/slider1-updated.jpg",
            Title = "Yozgi super chegirmalar!",
            Description = "Eng yaxshi narxlar faqat bugun!",
            RedirectUrl = "/products/sale",
            IsActive = true
        };
    }
}
