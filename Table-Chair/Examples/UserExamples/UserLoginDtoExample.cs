using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.UserDtos;

namespace Table_Chair.Examples.UserExamples
{
    public class UserLoginDtoExample:IExamplesProvider<UserLoginDto>
    {
        public UserLoginDto GetExamples()
        {
            return new UserLoginDto()
            {
                Login = "ali@example.com or username", 
                Password = "P@ssw0rd!",
                RememberMe = true
            };
        }
    }
}
