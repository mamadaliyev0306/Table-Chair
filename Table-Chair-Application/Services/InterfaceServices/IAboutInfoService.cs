using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AboutInfoDtos;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IAboutInfoService
    {
        Task<IEnumerable<AboutInfoDto>> GetAllAsync();
        Task<AboutInfoDto> GetByIdAsync(int id);
        Task CreateAsync(AboutInfoCreateDto dto);
        Task UpdateAsync(int id, AboutInfoCreateDto dto);
        Task DeleteAsync(int id);
    }

}
