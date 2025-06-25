using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;
using Table_Chair_Application.Exceptions;

namespace Table_Chair_Application.Services
{
    public class SliderService : ISliderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SliderService> _logger;

        public SliderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SliderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // Add a new Slider
        public async Task AddSliderAsync(CreateSliderDto sliderDto)
        {
            try
            {
                if (sliderDto == null)
                {
                    _logger.LogError("AddSliderAsync failed: SliderDto is null.");
                    throw new ArgumentNullException(nameof(sliderDto), "SliderDto cannot be null.");
                }

                var sliderEntity = _mapper.Map<Slider>(sliderDto);
                await _unitOfWork.Sliders.AddAsync(sliderEntity);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Slider with Id {sliderEntity.Id} has been added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the slider.");
                throw new ApplicationException("An error occurred while adding the slider.", ex);
            }
        }

        // Delete Slider by Id
        public async Task DeleteSliderAsync(int id)
        {
            try
            {
                var slider = await _unitOfWork.Sliders.GetByIdAsync(id);
                if (slider == null)
                {
                    _logger.LogWarning($"Slider with Id {id} not found.");
                    throw new NotFoundException($"Slider with Id {id} not found.");
                }

                _unitOfWork.Sliders.Delete(slider);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Slider with Id {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the slider.");
                throw new ApplicationException("An error occurred while deleting the slider.", ex);
            }
        }

        // Get Slider by Id
        public async Task<SliderDto> GetSliderByIdAsync(int id)
        {
            try
            {
                var slider = await _unitOfWork.Sliders.GetByIdAsync(id);
                if (slider == null)
                {
                    _logger.LogWarning($"Slider with Id {id} not found.");
                    throw new NotFoundException($"Slider with Id {id} not found.");
                }

                _logger.LogInformation($"Slider with Id {id} has been retrieved successfully.");
                return _mapper.Map<SliderDto>(slider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the slider.");
                throw new ApplicationException("An error occurred while retrieving the slider.", ex);
            }
        }

        // Get list of Sliders
        public async Task<IEnumerable<SliderDto>> GetSliderListAsync()
        {
            try
            {
                var sliders = await _unitOfWork.Sliders.GetAllAsync();
                if (sliders == null || !sliders.Any())
                {
                    _logger.LogWarning("No sliders found.");
                    throw new NotFoundException("No sliders found.");
                }

                _logger.LogInformation("Sliders list has been retrieved successfully.");
                return _mapper.Map<IEnumerable<SliderDto>>(sliders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the list of sliders.");
                throw new ApplicationException("An error occurred while retrieving the list of sliders.", ex);
            }
        }

        // Update an existing Slider
        public async Task UpdateSliderAsync(SliderUpdateDto sliderDto)
        {
            try
            {
                if (sliderDto == null)
                {
                    _logger.LogError("UpdateSliderAsync failed: SliderDto is null.");
                    throw new ArgumentNullException(nameof(sliderDto), "SliderDto cannot be null.");
                }

                var slider = await _unitOfWork.Sliders.GetByIdAsync(sliderDto.Id);
                if (slider == null)
                {
                    _logger.LogWarning($"Slider with Id {sliderDto.Id} not found.");
                    throw new NotFoundException($"Slider with Id {sliderDto.Id} not found.");
                }

                // Mapping only the properties that need to be updated
                _mapper.Map(sliderDto, slider);
                _unitOfWork.Sliders.Update(slider);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Slider with Id {sliderDto.Id} has been updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the slider.");
                throw new ApplicationException("An error occurred while updating the slider.", ex);
            }
        }
    }
}

