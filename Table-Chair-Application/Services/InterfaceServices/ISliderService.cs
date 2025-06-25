using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface ISliderService
    {
        Task<IEnumerable<SliderDto>> GetSliderListAsync();
        Task<SliderDto> GetSliderByIdAsync(int id);
        Task AddSliderAsync(CreateSliderDto sliderDto);
        Task UpdateSliderAsync(SliderUpdateDto sliderDto);
        Task DeleteSliderAsync(int id);
    }
}
