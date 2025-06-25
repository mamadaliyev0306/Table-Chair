using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.BlogDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services;

public class BlogService : IBlogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BlogService> _logger;

    public BlogService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BlogService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<BlogDto>> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Blogs.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted)
            throw new NotFoundException($"Blog ID {id} topilmadi.");

        var dto = _mapper.Map<BlogDto>(entity);
        return ApiResponse<BlogDto>.SuccessResponse(dto, "Blog topildi");
    }

    public async Task<ApiResponse<IEnumerable<BlogDto>>> GetAllAsync()
    {
        var entities = await _unitOfWork.Blogs.GetAllAsync();
        var filtered = entities.Where(b => !b.IsDeleted);
        var dtos = _mapper.Map<IEnumerable<BlogDto>>(filtered);
        return ApiResponse<IEnumerable<BlogDto>>.SuccessResponse(dtos, "Barcha bloglar");
    }

    public async Task<ApiResponse<string>> CreateAsync(BlogCreateDto blogCreateDto)
    {
        var entity = _mapper.Map<Blog>(blogCreateDto);
        entity.PublishedAt = DateTime.UtcNow;

        await _unitOfWork.Blogs.AddAsync(entity);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Blog muvaffaqiyatli yaratildi.");
        return ApiResponse<string>.SuccessResponse("Blog yaratildi");
    }

    public async Task<ApiResponse<string>> UpdateAsync(int id, BlogUpdateDto blogUpdateDto)
    {
        var entity = await _unitOfWork.Blogs.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted)
            throw new NotFoundException($"Blog ID {id} topilmadi.");

        _mapper.Map(blogUpdateDto, entity);
        entity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Blogs.Update(entity);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Blog yangilandi. ID: {BlogId}", id);
        return ApiResponse<string>.SuccessResponse("Blog yangilandi");
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Blogs.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted)
            throw new NotFoundException($"Blog ID {id} topilmadi.");

        // Soft delete
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Blogs.Update(entity);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Blog soft delete qilindi. ID: {BlogId}", id);
        return ApiResponse<string>.SuccessResponse("Blog o‘chirildi");
    }
}

