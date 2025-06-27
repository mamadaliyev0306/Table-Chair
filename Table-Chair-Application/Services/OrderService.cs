// OrderService: Real-world optimized version
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDto> AddAsync(CreateOrderDto orderDto)
        {
            if (orderDto == null) throw new ArgumentNullException(nameof(orderDto));

            var order = _mapper.Map<Order>(orderDto);
            order.CreatedAt = order.UpdatedAt = DateTime.UtcNow;

            order.OrderItems = orderDto.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Order created successfully. Id: {OrderId}", order.Id);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderFromCartAsync(int userId, CheckoutDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                       ?? throw new NotFoundException($"User {userId} not found.");

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddressId = dto.ShippingAddressId,
                    PaymentMethod = dto.PaymentMethod,
                    Status = OrderStatus.Created,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OrderItems = dto.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.Carts.ClearCartAsync(userId);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order created from cart for UserId: {UserId}", userId);
                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create order from cart for UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _unitOfWork.Orders.GetAll()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetAll()
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetAll()
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new NotFoundException("Order not found.");

            return _mapper.Map<IEnumerable<OrderItemDto>>(order.OrderItems);
        }

        public async Task<OrderItemDto> AddItemToOrderAsync(int orderId, OrderItemCreateDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
                        ?? throw new NotFoundException("Order not found.");

            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId)
                          ?? throw new NotFoundException("Product not found.");

            var orderItem = _mapper.Map<OrderItem>(dto);
            order.OrderItems.Add(orderItem);
            order.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderItemDto>(orderItem);
        }

        public async Task UpdateOrderItemAsync(int orderId, int itemId, OrderItemCreateDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
                        ?? throw new NotFoundException("Order not found.");

            var item = order.OrderItems.FirstOrDefault(i => i.Id == itemId)
                       ?? throw new NotFoundException("Order item not found.");

            _mapper.Map(dto, item);
            order.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();
        }

        public async Task RemoveItemFromOrderAsync(int orderId, int itemId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
                        ?? throw new NotFoundException("Order not found.");

            var item = order.OrderItems.FirstOrDefault(i => i.Id == itemId)
                       ?? throw new NotFoundException("Order item not found.");

            order.OrderItems.Remove(item);
            order.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(OrderUpdateDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(dto.Id)
                        ?? throw new NotFoundException($"Order {dto.Id} not found.");

            _mapper.Map(dto, order);
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
                        ?? throw new NotFoundException("Order not found.");

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
        }

        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
                        ?? throw new NotFoundException("Order not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order already cancelled.");

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<decimal> GetTotalPriceAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetAll()
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new NotFoundException("Order not found.");

            return order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);
        }

        public async Task<IEnumerable<OrderDto>> GetLatestOrdersAsync(int count = 10)
        {
            var orders = await _unitOfWork.Orders.GetAll()
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public IQueryable<OrderDto> GetOrdersByDateRange(DateTime start, DateTime end)
        {
            if (start > end) throw new ArgumentException("Invalid date range.");

            var query = _unitOfWork.Orders.GetAll()
                .Include(o => o.OrderItems)
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end);

            return _mapper.ProjectTo<OrderDto>(query);
        }

        public IQueryable<OrderDto> GetUserOrders(int userId)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException();

            var query = _unitOfWork.Orders.GetAll()
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId);

            return _mapper.ProjectTo<OrderDto>(query);
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id)
                        ?? throw new NotFoundException($"Order {id} not found.");

            _unitOfWork.Orders.Delete(order);
            await _unitOfWork.CompleteAsync();
        }
    }
}
