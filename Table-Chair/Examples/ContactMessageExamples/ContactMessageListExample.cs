using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos;

public class ContactMessageListExample : IExamplesProvider<List<ContactMessageDto>>
{
    public List<ContactMessageDto> GetExamples()
    {
        return new List<ContactMessageDto>
        {
            new ContactMessageDto
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
            },
            new ContactMessageDto
            {
                Id = 2,
                FirstName = "Nodira",
                LastName = "Toshpulatova",
                Email = "nodira.t@example.com",
                Message = "Rahmat sizlarga! Juda foydali xizmat!",
                SentAt = DateTime.UtcNow.AddDays(-1),
                IsRead = true,
                IsResponded = true,
                PhoneNumber = "+998938888888"
            }
        };
    }
}
