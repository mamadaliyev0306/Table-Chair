using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos;

public class UpdateTestimonialDtoExample : IExamplesProvider<UpdateTestimonialDto>
{
    public UpdateTestimonialDto GetExamples()
    {
        return new UpdateTestimonialDto
        {
            Id = 1,
            AuthorName = "Jasur Abdullayev",
            Content = "Saytdan ikkinchi marta foydalandim – yana ham yaxshiroq xizmat!",
            ImageUrl = "https://cdn.example.com/images/users/user1-updated.jpg"
        };
    }
}
