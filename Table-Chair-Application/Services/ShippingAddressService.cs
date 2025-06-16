using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.ShippingAddressDtos;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class ShippingAddressService : IShippingAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ShippingAddressService> _logger;
        private readonly IMemoryCache _cache;

        public ShippingAddressService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ShippingAddressService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task CreateAsync(ShippingAddressCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                var shippingAddress = _mapper.Map<ShippingAddress>(dto);
                await _unitOfWork.ShippingAddresses.AddAsync(shippingAddress);
                await _unitOfWork.CompleteAsync();

                // Log successful creation
                _logger.LogInformation("Shipping address created successfully with ID: {ShippingAddressId}", shippingAddress.Id);

                // Clear cache if needed
                _cache.Remove("ShippingAddressesCache");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating shipping address.");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.ShippingAddresses.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Shipping address with ID {ShippingAddressId} not found for deletion.", id);
                    return false;
                }

                _unitOfWork.ShippingAddresses.Delete(result);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Shipping address with ID {ShippingAddressId} deleted successfully.", id);

                // Clear cache if needed
                _cache.Remove("ShippingAddressesCache");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting shipping address with ID {ShippingAddressId}.", id);
                throw;
            }
        }

        public async Task<ShippingAddressDto> GetByIdAsync(int id)
        {
            try
            {
                // Check cache first
                if (!_cache.TryGetValue($"ShippingAddress_{id}", out ShippingAddressDto? cachedAddress)) // Use nullable type
                {
                    var result = await _unitOfWork.ShippingAddresses.GetByIdAsync(id);
                    if (result == null)
                    {
                        _logger.LogWarning("Shipping address with ID {ShippingAddressId} not found.", id);
                        throw new KeyNotFoundException("Shipping address not found.");
                    }

                    cachedAddress = _mapper.Map<ShippingAddressDto>(result);

                    // Cache the result for 5 minutes
                    _cache.Set($"ShippingAddress_{id}", cachedAddress, TimeSpan.FromMinutes(5));

                    _logger.LogInformation("Shipping address with ID {ShippingAddressId} fetched and cached.", id);
                }

                return cachedAddress!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching shipping address with ID {ShippingAddressId}.", id);
                throw;
            }
        }


        public async Task<bool> UpdateAsync(int id, ShippingAddressUpdateDto dto)
        {
            try
            {
                var result = await _unitOfWork.ShippingAddresses.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Shipping address with ID {ShippingAddressId} not found for update.", id);
                    throw new KeyNotFoundException("Shipping address not found.");
                }

                var entity = _mapper.Map(dto, result);
                _unitOfWork.ShippingAddresses.Update(entity);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Shipping address with ID {ShippingAddressId} updated successfully.", id);

                // Clear cache if needed
                _cache.Remove($"ShippingAddress_{id}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating shipping address with ID {ShippingAddressId}.", id);
                throw;
            }
        }
    }
}

