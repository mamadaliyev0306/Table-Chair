using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.CreateDtos;

public class CreateTestimonialDtoExample : IExamplesProvider<CreateTestimonialDto>
{
    public CreateTestimonialDto GetExamples()
    {
        return new CreateTestimonialDto
        {
            AuthorName = "Jasur Abdullayev",
            Content = "Bu sayt orqali xarid qilish juda qulay va tez. Juda mamnunman!",
            ImageUrl = "https://cdn.example.com/images/users/user1.jpg"
        };
    }
}
