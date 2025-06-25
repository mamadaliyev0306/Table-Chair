using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.AboutInfoDtos;

public class AboutInfoCreateDtoExample : IExamplesProvider<AboutInfoCreateDto>
{
    public AboutInfoCreateDto GetExamples()
    {
        return new AboutInfoCreateDto
        {
            Content = "Content.",
            ImageUrl = "https://example.com/image.png"
        };
    }
}
