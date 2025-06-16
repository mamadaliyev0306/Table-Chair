using AutoMapper;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactMessageService> _logger;

        public ContactMessageService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ContactMessageService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task CreateAsync(ContactMessageCreateDto dto)
        {
            try
            {
                if (dto == null)
                    throw new ValidationException("ContactMessageCreateDto must not be null");

                var entity = _mapper.Map<ContactMessage>(dto);
                entity.CreatedAt= DateTime.UtcNow;
                entity.UpdatedAt= DateTime.UtcNow;
                await _unitOfWork.ContactMessages.AddAsync(entity);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Contact message created successfully. Id: {Id}", entity.Id);
            }
            catch (CustomException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CreateAsync");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ValidationException("Invalid Id");

                var existingMessage = await _unitOfWork.ContactMessages.GetByIdAsync(id);
                if (existingMessage == null)
                    throw new NotFoundException($"Contact message with Id {id} not found.");

                _unitOfWork.ContactMessages.Delete(existingMessage);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Contact message deleted successfully. Id: {Id}", id);
            }
            catch (CustomException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DeleteAsync");
                throw;
            }
        }

        public async Task<List<ContactMessageDto>> GetAllAsync()
        {
            try
            {
                var messages = await _unitOfWork.ContactMessages.GetAllAsync();
                if (messages == null || !messages.Any())
                {
                    _logger.LogWarning("No contact messages found.");
                    return new List<ContactMessageDto>();
                }

                _logger.LogInformation("Contact messages retrieved successfully.");
                return _mapper.Map<List<ContactMessageDto>>(messages);
            }
            catch (CustomException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetAllAsync");
                throw;
            }
        }

        public async Task<ContactMessageDto?> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ValidationException("Invalid Id");

                var message = await _unitOfWork.ContactMessages.GetByIdAsync(id);
                if (message == null)
                    throw new NotFoundException($"Contact message with Id {id} not found.");

                _logger.LogInformation("Contact message with Id {Id} retrieved successfully.", id);
                return _mapper.Map<ContactMessageDto>(message);
            }
            catch (CustomException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetByIdAsync");
                throw;
            }
        }
    }
}

