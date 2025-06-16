using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IContactMessageService
    {
        Task<List<ContactMessageDto>> GetAllAsync();
        Task<ContactMessageDto?> GetByIdAsync(int id);
        Task CreateAsync(ContactMessageCreateDto dto);
        Task DeleteAsync(int id);
    }

}
