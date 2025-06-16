using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class FaqService : IFaqService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FaqService> _logger;

        public FaqService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<FaqService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task CreateAsync(FaqCreateDto faqCreateDto)
        {
            if (faqCreateDto == null)
            {
                _logger.LogWarning("Faq create DTO is null.");
                throw new ArgumentNullException(nameof(faqCreateDto));
            }

            var faq = _mapper.Map<Faq>(faqCreateDto);
            faq.CreatedAt = DateTime.UtcNow;
            
            await _unitOfWork.Faqs.AddAsync(faq);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Faq created successfully with Question: ");
        }

        public async Task UpdateAsync(int id, FaqUpdateDto faqUpdateDto)
        {
            if (faqUpdateDto == null)
            {
                _logger.LogWarning("Faq update DTO is null.");
                throw new ArgumentNullException(nameof(faqUpdateDto));
            }

            var faq = await _unitOfWork.Faqs.GetByIdAsync(id);
            if (faq == null)
            {
                _logger.LogWarning($"Faq with ID {id} not found.");
                throw new NotFoundException($"Faq with Id {id} not found.");
            }

            _mapper.Map(faqUpdateDto, faq);
            _unitOfWork.Faqs.Update(faq);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Faq with ID {id} updated successfully.");
        }

        public async Task DeleteAsync(int id)
        {
            var faq = await _unitOfWork.Faqs.GetByIdAsync(id);
            if (faq == null)
            {
                _logger.LogWarning($"Faq with ID {id} not found for deletion.");
                throw new NotFoundException($"Faq with Id {id} not found.");
            }

            _unitOfWork.Faqs.Delete(faq);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Faq with ID {id} hard deleted successfully.");
        }

        public async Task<FaqDto> GetByIdAsync(int id)
        {
            var faq = await _unitOfWork.Faqs.GetByIdAsync(id);
            if (faq == null)
            {
                _logger.LogWarning($"Faq with ID {id} not found.");
                throw new NotFoundException($"Faq with Id {id} not found.");
            }

            _logger.LogInformation($"Faq with ID {id} retrieved successfully.");
            return _mapper.Map<FaqDto>(faq);
        }

        public async Task<IEnumerable<FaqDto>> GetAllAsync(
            Expression<Func<Faq, bool>>? filter = null,
            Func<IQueryable<Faq>, IOrderedQueryable<Faq>>? orderBy = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<Faq> query = _unitOfWork.Faqs.GetQueryable();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page.HasValue && pageSize.HasValue)
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            var faqs = await _unitOfWork.Faqs.ToListAsync(query);
            _logger.LogInformation($"Retrieved {faqs.Count} FAQs.");

            return _mapper.Map<IEnumerable<FaqDto>>(faqs);
        }

        public async Task<IEnumerable<FaqDto>> GetActiveAsync()
        {
            var activeFaqs = await _unitOfWork.Faqs.ToListAsync(
                _unitOfWork.Faqs.GetQueryable().Where(f => !f.IsDeleted));

            _logger.LogInformation($"Retrieved {activeFaqs.Count} active FAQs.");
            return _mapper.Map<IEnumerable<FaqDto>>(activeFaqs);
        }
        public async Task<int> CountAsync(Expression<Func<Faq, bool>>? filter = null)
        {
            IQueryable<Faq> query = _unitOfWork.Faqs.GetQueryable();

            if (filter != null)
                query = query.Where(filter);

            int count = await query.CountAsync(); // <-- Bu yer to'g'ri
            _logger.LogInformation($"FAQ count: {count}");

            return count;
        }


        public async Task<bool> ExistsAsync(Expression<Func<Faq, bool>> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            bool exists = await _unitOfWork.Faqs.AnyAsync(_unitOfWork.Faqs.GetQueryable().Where(filter));
            _logger.LogInformation($"Faq exists check returned: {exists}");

            return exists;
        }

        public async Task SoftDeleteAsync(int id)
        {
            var faq = await _unitOfWork.Faqs.GetByIdAsync(id);
            if (faq == null)
            {
                _logger.LogWarning($"Faq with ID {id} not found for soft delete.");
                throw new NotFoundException($"Faq with Id {id} not found.");
            }

            faq.IsDeleted = true;
            _unitOfWork.Faqs.Update(faq);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Faq with ID {id} soft deleted successfully.");
        }

        public async Task RestoreAsync(int id)
        {
            var faq = await _unitOfWork.Faqs.GetByIdAsync(id);
            if (faq == null)
            {
                _logger.LogWarning($"Faq with ID {id} not found for restore.");
                throw new NotFoundException($"Faq with Id {id} not found.");
            }

            faq.IsDeleted = false;
            _unitOfWork.Faqs.Update(faq);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Faq with ID {id} restored successfully.");
        }

        public async Task<IEnumerable<FaqDto>> DynamicFilterAsync(Dictionary<string, object> filters, int? page = null, int? pageSize = null)
        {
            try
            {
                IQueryable<Faq> query = _unitOfWork.Faqs.GetQueryable();

                foreach (var filter in filters)
                {
                    var property = typeof(Faq).GetProperty(filter.Key);
                    if (property != null)
                    {
                        query = query.Where(f => EF.Property<object>(f, filter.Key).Equals(filter.Value));
                    }
                }

                if (page.HasValue && pageSize.HasValue)
                    query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

                var result = await query.ToListAsync();
                _logger.LogInformation($"Dynamic filter applied. Retrieved {result.Count} FAQs.");

                return _mapper.Map<IEnumerable<FaqDto>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during dynamic filtering.");
                throw;
            }
        }

        public async Task<IEnumerable<FaqDto>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<FaqDto>();

            IQueryable<Faq> query = _unitOfWork.Faqs.GetQueryable()
                .Where(f => f.Title.Contains(searchTerm) || f.Answer.Contains(searchTerm));

            var results = await query.ToListAsync();
            _logger.LogInformation($"Search for '{searchTerm}' returned {results.Count} FAQs.");

            return _mapper.Map<IEnumerable<FaqDto>>(results);
        }
    }
}


