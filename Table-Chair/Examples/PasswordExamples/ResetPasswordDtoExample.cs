using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.UserDtos;

namespace Table_Chair.Examples.PasswordExamples
{
    public class ResetPasswordDtoExample : IExamplesProvider<ResetPasswordDto>
    {
        public ResetPasswordDto GetExamples()
        {
            return new ResetPasswordDto()
            {
                Email = "ali@gmail.com",
                Token = "ADF$6?jhg?/178",
                NewPassword = "asF9782&^%",
                ConfirmNewPassword = "asF9782&^%"
            };
        }
    }
}
