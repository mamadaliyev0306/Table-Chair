using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.BlogDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
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

        public async Task CreateAsync(BlogCreateDto blogCreateDto)
        {
            try
            {
                if (blogCreateDto == null)
                {
                    _logger.LogWarning("CreateAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(blogCreateDto));
                }

                var entity = _mapper.Map<Blog>(blogCreateDto);
                await _unitOfWork.Blogs.AddAsync(entity);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Blog created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a blog.");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Blogs.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning($"DeleteAsync called but Blog with Id {id} not found.");
                    throw new NotFoundException($"Blog with Id {id} not found.");
                }

                _unitOfWork.Blogs.Delete(entity);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Blog with Id {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting blog with Id {id}.");
                throw;
            }
        }

        public async Task<IEnumerable<BlogDto>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Blogs.GetAllAsync();
                _logger.LogInformation("Retrieved all blogs successfully.");
                return _mapper.Map<IEnumerable<BlogDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all blogs.");
                throw;
            }
        }

        public async Task<BlogDto> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Blogs.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning($"GetByIdAsync called but Blog with Id {id} not found.");
                    throw new NotFoundException($"Blog with Id {id} not found.");
                }

                _logger.LogInformation($"Retrieved blog with Id {id} successfully.");
                return _mapper.Map<BlogDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving blog with Id {id}.");
                throw;
            }
        }

        public async Task UpdateAsync(int id, BlogUpdateDto blogUpdateDto)
        {
            try
            {
                if (blogUpdateDto == null)
                {
                    _logger.LogWarning("UpdateAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(blogUpdateDto));
                }

                var entity = await _unitOfWork.Blogs.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning($"UpdateAsync called but Blog with Id {id} not found.");
                    throw new NotFoundException($"Blog with Id {id} not found.");
                }

                _mapper.Map(blogUpdateDto, entity); // To'g'ri: Dto -> Entity
                _unitOfWork.Blogs.Update(entity);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Blog with Id {id} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating blog with Id {id}.");
                throw;
            }
        }
    }
}

