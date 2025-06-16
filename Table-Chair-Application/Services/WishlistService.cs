using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WishlistService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AddAsync(int userId, WishlistItemCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new NotFoundException("null dto");
                }
                var map= _mapper.Map<WishlistItem>(dto);
                map.UserId = userId;
               await _unitOfWork.Wishlist.AddAsync(map);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Xatolik bor " + ex.Message);
                throw new NotFoundException(ex.Message);
            }
        }

        public async Task<bool> ExistsAsync(int userId, int productId)
        {
            try
            {
                var result = await _unitOfWork.Wishlist.ExistsAsync(userId, productId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<int> GetWishlistCountAsync(int userId)
        {
            try
            {
                var result = await _unitOfWork.Wishlist.CountAsync(userId);
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return 0;
            }
        }

        public async Task<IEnumerable<WishlistItemDto>> GetWishlistProductsAsync(int userId)
        {
            try
            {
                var result = await _unitOfWork.Wishlist.GetWishlistProductsAsync(userId);
                if (result == null)
                {
                    return Enumerable.Empty<WishlistItemDto>();
                }
                return _mapper.Map< IEnumerable<WishlistItemDto>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Enumerable.Empty<WishlistItemDto>();
            }
        }

        public async Task RemoveAsync(int userId, int productId)
        {
            try
            {
                 await _unitOfWork.Wishlist.RemoveAsync(userId, productId);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new NotFoundException(ex.Message);
            }
        }

        public async Task<WishlistToggleResultDto> ToggleWishlistAsync(int userId, int productId)
        {
            var item = await _unitOfWork.Wishlist.GetWishlistItemAsync(userId, productId);
            bool isInWishlist;

            if (item != null)
            {
                await _unitOfWork.Wishlist.RemoveAsync(item.UserId, productId);
                isInWishlist = false;
            }
            else
            {
                var userExists = await _unitOfWork.Users.ExistsUserIdAsync(userId);
                if (!userExists)
                {
                    throw new UnauthorizedAccessException("Foydalanuvchi topilmadi.");
                }
                var newItem = new WishlistItem
                {
                    UserId = userId,
                    ProductId = productId,
                    AddedAt = DateTime.UtcNow
                };
                await _unitOfWork.Wishlist.AddAsync(newItem);
                isInWishlist = true;
            }

            await _unitOfWork.CompleteAsync();

            return new WishlistToggleResultDto { IsInWishlist = isInWishlist };
        }
    }
    
}
