using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface IOrderRepository:IGenericRepository<Order>
    {
        IQueryable<Order> GetUserOrdersQuery(int userId);
        Task<Order> CreateOrderFromCartAsync(int userId, CheckoutDto checkoutDto);
        Task UpdateOrderStatusAsync(int orderId, string status);
        IQueryable<Order> GetOrdersByDateRangeQuery(DateTime startDate, DateTime endDate);
    }
}
