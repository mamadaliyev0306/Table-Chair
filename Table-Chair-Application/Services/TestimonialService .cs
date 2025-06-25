using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class TestimonialService : ITestimonialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TestimonialService> _logger;

        public TestimonialService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TestimonialService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task CreateAsync(CreateTestimonialDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("CreateTestimonialDto is null");
                    throw new ArgumentNullException(nameof(dto));
                }

                var mapp = _mapper.Map<Testimonial>(dto);
                await _unitOfWork.Testimons.AddAsync(mapp);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Testimonial created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating testimonial.");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.Testimons.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Testimonial with ID {TestimonialId} not found for deletion.", id);
                    return;
                }

                _unitOfWork.Testimons.Delete(result);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Testimonial with ID {TestimonialId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting testimonial with ID {TestimonialId}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<TestimonialDto>> GetAllAsync()
        {
            try
            {
                var result = await _unitOfWork.Testimons.GetAllAsync();
                var testimonialsDto = _mapper.Map<IEnumerable<TestimonialDto>>(result);

                _logger.LogInformation("Successfully retrieved all testimonials.");
                return testimonialsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all testimonials.");
                throw;
            }
        }

        public async Task<TestimonialDto> GetByIdAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.Testimons.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Testimonial with ID {TestimonialId} not found.", id);
                    throw new NotFoundException("Error");
                }

                var testimonialDto = _mapper.Map<TestimonialDto>(result);
                _logger.LogInformation("Successfully retrieved testimonial with ID {TestimonialId}.", id);
                return testimonialDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving testimonial with ID {TestimonialId}.", id);
                throw;
            }
        }

        public async Task UpdateAsync(UpdateTestimonialDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("CreateTestimonialDto is null for ID {TestimonialId}.",dto);
                    throw new ArgumentNullException(nameof(dto));
                }

                var result = await _unitOfWork.Testimons.GetByIdAsync(dto.Id);
                if (result == null)
                {
                    _logger.LogWarning("Testimonial with ID {TestimonialId} not found for update.", dto.Id);
                    return;
                }

                var entity = _mapper.Map(dto, result);
                _unitOfWork.Testimons.Update(entity);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Testimonial with ID {TestimonialId} updated successfully.", dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating testimonial with ID {TestimonialId}.", dto.Id);
                throw;
            }
        }

        // SoftDelete
        public async Task SoftDeleteAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.Testimons.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Testimonial with ID {TestimonialId} not found for soft delete.", id);
                    return;
                }

                await _unitOfWork.Testimons.SoftDeleteAsync(result);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Testimonial with ID {TestimonialId} soft deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while performing soft delete on testimonial with ID {TestimonialId}.", id);
                throw;
            }
        }

        // Restore
        public async Task RestoreAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.Testimons.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Testimonial with ID {TestimonialId} not found for restore.", id);
                    return;
                }

                await _unitOfWork.Testimons.RestoreAsync(result);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Testimonial with ID {TestimonialId} restored successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while restoring testimonial with ID {TestimonialId}.", id);
                throw;
            }
        }
    }
}

