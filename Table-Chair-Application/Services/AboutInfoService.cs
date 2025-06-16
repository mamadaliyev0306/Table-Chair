using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AboutInfoDtos;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class AboutInfoService : IAboutInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AboutInfoService> _logger;

        public AboutInfoService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AboutInfoService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task CreateAsync(AboutInfoCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("CreateAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto));
                }

                var entity = _mapper.Map<AboutInfo>(dto);
                
                await _unitOfWork.AboutInfos.AddAsync(entity);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("AboutInfo created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating AboutInfo.");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.AboutInfos.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning($"DeleteAsync called but AboutInfo with Id {id} not found.");
                    throw new KeyNotFoundException($"AboutInfo with Id {id} not found.");
                }

                _unitOfWork.AboutInfos.Delete(entity);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"AboutInfo with Id {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting AboutInfo with Id {id}.");
                throw;
            }
        }

        public async Task<IEnumerable<AboutInfoDto>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.AboutInfos.GetAllAsync();
                _logger.LogInformation("Retrieved all AboutInfos successfully.");
                return _mapper.Map<IEnumerable<AboutInfoDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all AboutInfos.");
                throw;
            }
        }

        public async Task<AboutInfoDto> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.AboutInfos.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning($"GetByIdAsync called but AboutInfo with Id {id} not found.");
                    throw new KeyNotFoundException($"AboutInfo with Id {id} not found.");
                }

                _logger.LogInformation($"Retrieved AboutInfo with Id {id} successfully.");
                return _mapper.Map<AboutInfoDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving AboutInfo with Id {id}.");
                throw;
            }
        }

        public async Task UpdateAsync(int id, AboutInfoCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("UpdateAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto));
                }

                var entity = await _unitOfWork.AboutInfos.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning($"UpdateAsync called but AboutInfo with Id {id} not found.");
                    throw new KeyNotFoundException($"AboutInfo with Id {id} not found.");
                }

                _mapper.Map(dto, entity);
                _unitOfWork.AboutInfos.Update(entity);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"AboutInfo with Id {id} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating AboutInfo with Id {id}.");
                throw;
            }
        }
    }
}

