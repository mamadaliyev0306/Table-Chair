using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CartService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CartDto>> GetUserCartsAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("User ID must be positive", nameof(userId));

                var query = _unitOfWork.Carts.GetQueryable(
                    filter: c => c.UserId == userId,
                    orderBy: q => q.OrderByDescending(c => c.CreatedAt),
                    includeProperties: "Items.Product" // Include Product information
                );

                var carts = await query.ToListAsync();
                return _mapper.Map<IEnumerable<CartDto>>(carts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting carts for user {UserId}", userId);
                throw;
            }
        }

        public async Task<CartDto> CreateCartForUserAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("User ID must be positive", nameof(userId));

                var existingCart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
                if (existingCart != null)
                {
                    _logger.LogWarning("User {UserId} already has an active cart", userId);
                    return _mapper.Map<CartDto>(existingCart);
                }

                var newCart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Items = new List<CartItem>()
                };

                await _unitOfWork.Carts.AddAsync(newCart);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Created new cart for user {UserId}", userId);
                return _mapper.Map<CartDto>(newCart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task<CartDto?> GetCartByUserIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("User ID must be positive", nameof(userId));

                var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
                if (cart == null)
                {
                    _logger.LogWarning("No cart found for user {UserId}", userId);
                    return null;
                }

                // Explicitly load items and their products
                await _unitOfWork.Carts.LoadItemsWithProductsAsync(cart.Id);

                return _mapper.Map<CartDto>(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<CartItemDto>> GetCartItemsAsync(int cartId)
        {
            try
            {
                if (cartId <= 0)
                    throw new ArgumentException("Cart ID must be positive", nameof(cartId));

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartId);
                if (cart == null)
                    throw new NotFoundException($"Cart with ID {cartId} not found");

                return _mapper.Map<List<CartItemDto>>(cart.Items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting items for cart {CartId}", cartId);
                throw;
            }
        }

        public async Task AddItemToCartAsync(int cartId, CartItemCreateDto itemDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (cartId <= 0)
                    throw new ArgumentException("Cart ID must be positive", nameof(cartId));

                if (itemDto == null)
                    throw new ArgumentNullException(nameof(itemDto));

                if (itemDto.Quantity <= 0)
                    throw new ArgumentException("Quantity must be positive", nameof(itemDto.Quantity));

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartId);
                if (cart == null)
                    throw new NotFoundException($"Cart with ID {cartId} not found");

                // Check product exists
                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product with ID {itemDto.ProductId} not found");

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += itemDto.Quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.CartItems.Update(existingItem);
                }
                else
                {
                    var cartItem = _mapper.Map<CartItem>(itemDto);
                    cartItem.CartId = cartId;
                    cartItem.CreatedAt = DateTime.UtcNow;
                    cartItem.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.CartItems.AddAsync(cartItem);
                }

                cart.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Carts.Update(cart);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Added item to cart {CartId}", cartId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateCartItemQuantityAsync(int cartItemId, int newQuantity)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (cartItemId <= 0)
                    throw new ArgumentException("Cart item ID must be positive", nameof(cartItemId));

                if (newQuantity <= 0)
                    throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

                var item = await _unitOfWork.Carts.GetItemByIdAsync(cartItemId);
                if (item == null)
                    throw new NotFoundException($"Cart item with ID {cartItemId} not found");

                if (item.Cart == null)
                    throw new InvalidOperationException($"Cart associated with item ID {cartItemId} is null");

                item.Quantity = newQuantity;
                item.UpdatedAt = DateTime.UtcNow;
                item.Cart.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.CartItems.Update(item);
                _unitOfWork.Carts.Update(item.Cart);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Updated quantity for cart item {CartItemId}", cartItemId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveItemFromCartAsync(int cartItemId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (cartItemId <= 0)
                    throw new ArgumentException("Cart item ID must be positive", nameof(cartItemId));

                var item = await _unitOfWork.Carts.GetItemByIdAsync(cartItemId);
                if (item == null)
                    throw new NotFoundException($"Cart item with ID {cartItemId} not found");

                await _unitOfWork.Carts.RemoveItemAsync(cartItemId);

                if (item.Cart != null)
                {
                    item.Cart.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Carts.Update(item.Cart);
                }

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Removed item {CartItemId} from cart", cartItemId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ClearCartAsync(int cartId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (cartId <= 0)
                    throw new ArgumentException("Cart ID must be positive", nameof(cartId));

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartId);
                if (cart == null)
                    throw new NotFoundException($"Cart with ID {cartId} not found");

                await _unitOfWork.Carts.ClearCartAsync(cartId);

                cart.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Carts.Update(cart);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Cleared cart {CartId}", cartId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CartExistsAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("User ID must be positive", nameof(userId));

                var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
                return cart != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if cart exists for user {UserId}", userId);
                throw;
            }
        }

        public async Task<decimal> CalculateCartTotalAsync(int cartId)
        {
            try
            {
                if (cartId <= 0)
                    throw new ArgumentException("Cart ID must be positive", nameof(cartId));

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartId);
                if (cart == null)
                    throw new NotFoundException($"Cart with ID {cartId} not found");

                return cart.Items.Sum(item => item.Quantity * (item.Product?.Price ?? 0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total for cart {CartId}", cartId);
                throw;
            }
        }

        public async Task<int> GetCartItemCountAsync(int cartId)
        {
            try
            {
                if (cartId <= 0)
                    throw new ArgumentException("Cart ID must be positive", nameof(cartId));

                var cart = await _unitOfWork.Carts.GetByIdWithItemsAsync(cartId);
                if (cart == null)
                    throw new NotFoundException($"Cart with ID {cartId} not found");

                return cart.Items.Sum(item => item.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item count for cart {CartId}", cartId);
                throw;
            }
        }
    }
}
