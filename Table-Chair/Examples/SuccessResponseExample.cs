using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Responses;

namespace Table_Chair.Examples
{
    public class SuccessResponseExample : IExamplesProvider<ApiResponse<string>>
    {
        public ApiResponse<string> GetExamples()
        {
            return ApiResponse<string>.SuccessResponse(string.Empty, "Amal muvaffaqiyatli bajarildi");
        }
    }

}
