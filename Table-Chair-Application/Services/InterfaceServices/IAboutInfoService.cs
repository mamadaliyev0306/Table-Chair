using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AboutInfoDtos;
using Table_Chair_Application.Responses;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IAboutInfoService
    {
        Task<ApiResponse<IEnumerable<AboutInfoDto>>> GetAllAsync();
        Task<ApiResponse<AboutInfoDto>> GetByIdAsync(int id);
        Task<ApiResponse<string>> CreateAsync(AboutInfoCreateDto dto);

        Task<ApiResponse<string>> UpdateAsync(int id, AboutInfoUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }

}
