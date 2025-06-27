using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly FurnitureDbContext _context;
        public OrderRepository(FurnitureDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderFromCartAsync(int userId, CheckoutDto checkoutDto)
        {
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                OrderItems = checkoutDto.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            return order;
        }


        public IQueryable<Order> GetOrdersByDateRangeQuery(DateTime startDate, DateTime endDate) =>
            _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .AsQueryable();

        public IQueryable<Order> GetUserOrdersQuery(int userId) =>
            _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .Include(o => o.StatusHistories)
                .AsQueryable();

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                var statusHistory = new OrderStatusHistory
                {
                    OrderId = orderId,
                    Status = order.Status,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = "System"
                };

                _context.Orders.Update(order);
            }
        }
    }
}
