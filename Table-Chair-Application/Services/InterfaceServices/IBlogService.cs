using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.BlogDtos;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IBlogService
    {
     Task<IEnumerable<BlogDto>> GetAllAsync();
     Task<BlogDto> GetByIdAsync(int id);
    Task CreateAsync(BlogCreateDto blogCreateDto);
     Task UpdateAsync(int id,BlogUpdateDto blogUpdateDto);
     Task DeleteAsync(int id);
    }
}
