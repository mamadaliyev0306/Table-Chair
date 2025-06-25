using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos;

public class ContactMessageExample : IExamplesProvider<ContactMessageDto>
{
    public ContactMessageDto GetExamples()
    {
        return new ContactMessageDto
        {
            Id = 1,
            FirstName = "Ali",
            LastName = "Karimov",
            Email = "ali.karimov@example.com",
            Message = "Salom, sayt haqida savolim bor edi.",
            SentAt = DateTime.UtcNow.AddHours(-2),
            IsRead = false,
            IsResponded = false,
            PhoneNumber = "+998901234567"
        };
    }
}
