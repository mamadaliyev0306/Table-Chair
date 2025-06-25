using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AboutInfoDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Responses;
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

        public async Task<ApiResponse<string>> CreateAsync(AboutInfoCreateDto dto)
        {
            if (dto == null)
                throw new ValidationException("DTO bo'sh bo'lishi mumkin emas.");

            var entity = _mapper.Map<AboutInfo>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsActive = true;
            entity.IsDeleted = false;

            await _unitOfWork.AboutInfos.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("AboutInfo yaratildi: {@Entity}", entity);
            return ApiResponse<string>.SuccessResponse("AboutInfo yaratildi");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.AboutInfos.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                throw new NotFoundException("AboutInfo");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            _unitOfWork.AboutInfos.Update(entity);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<string>.SuccessResponse("AboutInfo soft delete qilindi.");
        }

        public async Task<ApiResponse<IEnumerable<AboutInfoDto>>> GetAllAsync()
        {
            var list = await _unitOfWork.AboutInfos.GetAllAsync();
            var data = _mapper.Map<IEnumerable<AboutInfoDto>>(list);
            return ApiResponse<IEnumerable<AboutInfoDto>>.SuccessResponse(data);
        }

        public async Task<ApiResponse<AboutInfoDto>> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.AboutInfos.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                throw new NotFoundException("AboutInfo");

            var dto = _mapper.Map<AboutInfoDto>(entity);
            return ApiResponse<AboutInfoDto>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<string>> UpdateAsync(int id,AboutInfoUpdateDto dto)
        {
            if (dto == null)
                throw new ValidationException("DTO bo'sh bo'lishi mumkin emas.");

            var entity = await _unitOfWork.AboutInfos.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                throw new NotFoundException("AboutInfo");

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AboutInfos.Update(entity);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<string>.SuccessResponse("AboutInfo muvaffaqiyatli yangilandi.");
        }
    }
}

