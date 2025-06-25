using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.BlogDtos;
using Table_Chair_Application.Responses;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IBlogService
    {
     Task<ApiResponse<IEnumerable<BlogDto>>> GetAllAsync();
     Task<ApiResponse<BlogDto>> GetByIdAsync(int id);
     Task<ApiResponse<string>> CreateAsync(BlogCreateDto blogCreateDto);
     Task<ApiResponse<string>> UpdateAsync(int id, BlogUpdateDto blogUpdateDto);
     Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
