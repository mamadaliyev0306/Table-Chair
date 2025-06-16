using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.DetailsDtos;
using Table_Chair_Entity.Models;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(int id);
        Task<OrderDto> AddAsync(CreateOrderDto orderDto);
        Task UpdateAsync(OrderDto orderDto);
        Task DeleteAsync(int id);

        IQueryable<OrderDto> GetUserOrders(int userId);
        Task<OrderDto> CreateOrderFromCartAsync(int userId, CheckoutDto checkoutDto);
        Task UpdateOrderItemAsync(int orderId, int itemId, OrderItemCreateDto itemDto);
        IQueryable<OrderDto> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<OrderDto>> GetLatestOrdersAsync(int count = 10);
        Task<decimal> GetTotalPriceAsync(int orderId);
        Task CancelOrderAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }

}
