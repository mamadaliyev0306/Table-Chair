using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface ITestimonialService
    {
        Task<IEnumerable<TestimonialDto>> GetAllAsync();
        Task<TestimonialDto> GetByIdAsync(int id);
        Task CreateAsync(CreateTestimonialDto dto);
        Task UpdateAsync(UpdateTestimonialDto);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
        Task SoftDeleteAsync(int id);
    }

}
