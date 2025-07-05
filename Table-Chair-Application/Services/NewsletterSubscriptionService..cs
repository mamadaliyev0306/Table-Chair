using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class NewsletterSubscriptionService : INewsletterSubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NewsletterSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Add
        public async Task AddNewsletterSubscription(NewsletterSubscriptionCreateDto newsletterSubscriptionDto)
        {
            if (newsletterSubscriptionDto == null)
                throw new ArgumentNullException(nameof(newsletterSubscriptionDto));

            var entity = _mapper.Map<NewsletterSubscription>(newsletterSubscriptionDto);
            entity.SubscribedAt = DateTime.UtcNow;
            await _unitOfWork.NewsletterSubscriptions.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
        }

        // Get All
        public async Task<IEnumerable<NewsletterSubscriptionDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.NewsletterSubscriptions.GetAllAsync();
            return _mapper.Map<IEnumerable<NewsletterSubscriptionDto>>(entities);
        }

        // Get By Id
        public async Task<NewsletterSubscriptionDto> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.NewsletterSubscriptions.GetByIdAsync(id);
            if (entity == null)
                throw new Exception($"NewsletterSubscription with ID {id} not found.");

            return _mapper.Map<NewsletterSubscriptionDto>(entity);
        }

        // Update
        public async Task UpdateAsync(NewsletterSubscriptionUpdateDto newsletterSubscriptionDto)
        {
            if (newsletterSubscriptionDto == null)
                throw new ArgumentNullException(nameof(newsletterSubscriptionDto));

            var existingEntity = await _unitOfWork.NewsletterSubscriptions.GetByIdAsync(newsletterSubscriptionDto.Id);
            if (existingEntity == null)
                throw new Exception($"NewsletterSubscription with ID {newsletterSubscriptionDto.Id} not found.");

            _mapper.Map(newsletterSubscriptionDto, existingEntity); // mapping into existing entity
            existingEntity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.NewsletterSubscriptions.Update(existingEntity);
            await _unitOfWork.CompleteAsync();
        }

        // Hard Delete
        public async Task DeleteNewsletterSubscriptionAsync(int id)
        {
            var entity = await _unitOfWork.NewsletterSubscriptions.GetByIdAsync(id);
            if (entity == null)
                throw new Exception($"NewsletterSubscription with ID {id} not found.");

            _unitOfWork.NewsletterSubscriptions.Delete(entity);
            await _unitOfWork.CompleteAsync();
        }

        // Soft Delete
        public async Task SoftDeleteNewsletterSubscriptionAsync(int id)
        {
            var entity = await _unitOfWork.NewsletterSubscriptions.GetByIdAsync(id);
            if (entity == null)
                throw new Exception($"NewsletterSubscription with ID {id} not found.");

            await _unitOfWork.NewsletterSubscriptions.SoftDeleteAsync(entity);
            await _unitOfWork.CompleteAsync();
        }

        // Restore
        public async Task RestoreNewsletterSubscriptionAsync(int id)
        {
            var entity = await _unitOfWork.NewsletterSubscriptions.GetByIdIncludingDeletedAsync(id);
            if (entity == null)
                throw new Exception($"NewsletterSubscription with ID {id} not found.");

            await _unitOfWork.NewsletterSubscriptions.RestoreAsync(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}

