using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos.DetailsDtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IOrderStatusHistoryService
    {
        Task<IEnumerable<OrderStatusHistoryDto>> GetAllAsync();
        Task<OrderStatusHistoryDto> GetById(int id);
        Task AddOrderStatusHistory(CreateOrderStatusHistoryDto orderStatusHistoryDto);
        Task UpdateOrderStatusHistory(OrderStatusHistoryDto orderStatusHistoryDto);
        Task DeleteOrderStatusHistory(int id);
    }
}
