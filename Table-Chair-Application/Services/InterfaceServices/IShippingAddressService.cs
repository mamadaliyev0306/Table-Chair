using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.ShippingAddressDtos;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IShippingAddressService
    {
        Task<ShippingAddressDto> GetByIdAsync(int id);
        Task CreateAsync(ShippingAddressCreateDto dto);
        Task<bool> UpdateAsync(int id, ShippingAddressUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
