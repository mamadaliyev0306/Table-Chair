using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;

public class NewsletterSubscriptionDtoExample : IExamplesProvider<NewsletterSubscriptionDto>
{
    public NewsletterSubscriptionDto GetExamples()
    {
        return new NewsletterSubscriptionDto
        {
            Id = 1,
            Email = "johndoe@example.com",
            IsActive = true,
            SubscribedAt= DateTime.UtcNow
        };
    }
}

public class NewsletterSubscriptionCreateDtoExample : IExamplesProvider<NewsletterSubscriptionCreateDto>
{
    public NewsletterSubscriptionCreateDto GetExamples()
    {
        return new NewsletterSubscriptionCreateDto
        {
            Email = "janedoe@example.com"
        };
    }
}