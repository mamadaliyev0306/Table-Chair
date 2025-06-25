using AutoMapper;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;
using ValidationException = Table_Chair_Application.Exceptions.ValidationException;

namespace Table_Chair_Application.Services
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactMessageService> _logger;

        public ContactMessageService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ContactMessageService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateAsync(ContactMessageCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = _mapper.Map<ContactMessage>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ContactMessages.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Contact message created successfully. Id: {Id}", entity.Id);
        }

        public async Task DeleteAsync(int id)
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

        public async Task<List<ContactMessageDto>> GetAllAsync()
        {
            var messages = await _unitOfWork.ContactMessages.GetAllAsync();

            if (messages == null || !messages.Any())
            {
                _logger.LogInformation("No contact messages found.");
                return new List<ContactMessageDto>();
            }

            _logger.LogInformation("{Count} contact messages retrieved successfully.", messages.Count());
            return _mapper.Map<List<ContactMessageDto>>(messages);
        }

        public async Task<ContactMessageDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Invalid Id");

            var message = await _unitOfWork.ContactMessages.GetByIdAsync(id);
            if (message == null)
                throw new NotFoundException($"Contact message with Id {id} not found.");

            _logger.LogInformation("Contact message with Id {Id} retrieved successfully.", id);
            return _mapper.Map<ContactMessageDto>(message);
        }
    }
}
