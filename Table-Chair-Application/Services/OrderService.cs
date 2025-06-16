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
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));

            try
            {
                var order = _mapper.Map<Order>(orderDto);
                order.CreatedAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;
                

                // OrderItem-larni qo'shish
                order.OrderItems = orderDto.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList();

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new order.");
                throw;
            }
        }

        public async Task<OrderDto> CreateOrderFromCartAsync(int userId, CheckoutDto checkoutDto)
        {
            if (checkoutDto == null)
                throw new ArgumentNullException(nameof(checkoutDto));

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new NotFoundException($"User with id {userId} not found.");

                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // Yangi order yaratish
                    var order = new Order
                    {
                        UserId = userId,
                        ShippingAddressId = checkoutDto.ShippingAddressId,
                        PaymentMethod = checkoutDto.PaymentMethod,
                        Status = OrderStatus.Created,
                        CreatedAt = DateTime.UtcNow,
                        OrderItems = checkoutDto.Items.Select(item => new OrderItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        }).ToList()
                    };

                    await _unitOfWork.Orders.AddAsync(order);
                    await _unitOfWork.CompleteAsync();
                    await _unitOfWork.Carts.ClearCartAsync(userId);

                    await transaction.CommitAsync();

                    return _mapper.Map<OrderDto>(order);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating an order from cart for user {UserId}.", userId);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive.");

            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(id);
                if (order == null)
                    throw new NotFoundException($"Order with id {id} not found.");

                _unitOfWork.Orders.Delete(order);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting order with id {Id}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAll()
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all orders.");
                throw;
            }
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive.");

            try
            {
                var order = await _unitOfWork.Orders.GetAll()
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                    return null;

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving order by id {Id}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentOutOfRangeException(nameof(orderId));

            try
            {
                var order = await _unitOfWork.Orders.GetAll()
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    throw new NotFoundException("Order not found");

                return _mapper.Map<IEnumerable<OrderItemDto>>(order.OrderItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving order items for order {OrderId}.", orderId);
                throw;
            }
        }

        public async Task<OrderItemDto> AddItemToOrderAsync(int orderId, OrderItemCreateDto itemDto)
        {
            if (orderId <= 0) throw new ArgumentOutOfRangeException(nameof(orderId));
            if (itemDto == null) throw new ArgumentNullException(nameof(itemDto));
            if (itemDto.Quantity <= 0) throw new ArgumentException("Quantity must be positive");

            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null) throw new NotFoundException("Order not found");

                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
                if (product == null) throw new NotFoundException("Product not found");

                var orderItem = _mapper.Map<OrderItem>(itemDto);
                order.OrderItems.Add(orderItem);
                order.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();

                return _mapper.Map<OrderItemDto>(orderItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding item to order {OrderId}.", orderId);
                throw;
            }
        }

        public async Task UpdateOrderItemAsync(int orderId, int itemId, OrderItemCreateDto itemDto)
        {
            if (orderId <= 0) throw new ArgumentOutOfRangeException(nameof(orderId));
            if (itemId <= 0) throw new ArgumentOutOfRangeException(nameof(itemId));
            if (itemDto == null) throw new ArgumentNullException(nameof(itemDto));

            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null) throw new NotFoundException("Order not found");

                var item = order.OrderItems.FirstOrDefault(i => i.Id == itemId);
                if (item == null) throw new NotFoundException("Order item not found");

                _mapper.Map(itemDto, item);
                order.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating item {ItemId} in order {OrderId}.", itemId, orderId);
                throw;
            }
        }

        public async Task RemoveItemFromOrderAsync(int orderId, int itemId)
        {
            if (orderId <= 0) throw new ArgumentOutOfRangeException(nameof(orderId));
            if (itemId <= 0) throw new ArgumentOutOfRangeException(nameof(itemId));

            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null) throw new NotFoundException("Order not found");

                var item = order.OrderItems.FirstOrDefault(i => i.Id == itemId);
                if (item == null) throw new NotFoundException("Order item not found");

                order.OrderItems.Remove(item);
                order.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing item {ItemId} from order {OrderId}.", itemId, orderId);
                throw;
            }
        }

        public IQueryable<OrderDto> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date must be before end date.");

            try
            {
                var ordersQuery = _unitOfWork.Orders.GetAll()
                    .Include(o => o.OrderItems)
                    .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate);

                return _mapper.ProjectTo<OrderDto>(ordersQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders between {StartDate} and {EndDate}.", startDate, endDate);
                throw;
            }
        }

        public IQueryable<OrderDto> GetUserOrders(int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId), "UserId must be positive.");

            try
            {
                var userOrdersQuery = _unitOfWork.Orders.GetAll()
                    .Include(o => o.OrderItems)
                    .Where(o => o.UserId == userId);

                return _mapper.ProjectTo<OrderDto>(userOrdersQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders for user {UserId}.", userId);
                throw;
            }
        }

        public async Task UpdateAsync(OrderDto orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));

            try
            {
                var existingOrder = await _unitOfWork.Orders.GetByIdAsync(orderDto.Id);
                if (existingOrder == null)
                    throw new NotFoundException($"Order with id {orderDto.Id} not found.");

                _mapper.Map(orderDto, existingOrder);
                existingOrder.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Orders.Update(existingOrder);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating order with id {Id}.", orderDto.Id);
                throw;
            }
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            if (orderId <= 0)
                throw new ArgumentOutOfRangeException(nameof(orderId), "Order ID must be positive.");

            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                    throw new NotFoundException($"Order with id {orderId} not found.");

                order.Status = status;
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status of order {OrderId}.", orderId);
                throw;
            }
        }

        public async Task CancelOrderAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentOutOfRangeException(nameof(orderId), "Order ID must be positive.");

            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                    throw new NotFoundException($"Order with id {orderId} not found.");

                if (order.Status == OrderStatus.Cancelled)
                    throw new InvalidOperationException("Order is already cancelled.");

                order.Status = OrderStatus.Cancelled;
                order.CancelledAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Orders.Update(order);

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cancelling order {OrderId}.", orderId);
                throw;
            }
        }

        public async Task<decimal> GetTotalPriceAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentOutOfRangeException(nameof(orderId), "Order ID must be positive.");

            try
            {
                var order = await _unitOfWork.Orders.GetAll()
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    throw new NotFoundException($"Order with id {orderId} not found.");

                return order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating total price for order {OrderId}.", orderId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetLatestOrdersAsync(int count = 10)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive.");

            try
            {
                var latestOrders = await _unitOfWork.Orders.GetAll()
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(count)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<OrderDto>>(latestOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving latest {Count} orders.", count);
                throw;
            }
        }
    }
}

