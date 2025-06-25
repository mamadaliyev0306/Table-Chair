using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.UserDtos;

namespace Table_Chair.Examples.UserExamples
{
    public class UserRegisterDtoExample : IExamplesProvider<UserRegisterDto>
    {
        public UserRegisterDto GetExamples()
        {
            return new UserRegisterDto()
            {
                FirstName ="ali",
                LastName ="aliyev",
                Username = "ali03",
                Email ="aliyev@gmail.com",
                PhoneNumber ="998940031432",
                Password="Pa07@??2",
                ConfirmPassword= "Pa07@??2"
            };
        }
    }
}
