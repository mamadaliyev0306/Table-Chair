using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CategoryDto> AddAsync(CategoryCreateDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto));

            var category = _mapper.Map<Category>(categoryDto);
            category.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid category ID", nameof(id));

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return false;

            if (await _unitOfWork.Categories.HasProductsAsync(id))
                throw new InvalidOperationException("Cannot delete category with existing products");

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid category ID", nameof(id));

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found");

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<IEnumerable<CategoryWithProductsDto>> GetWithProductsAsync()
        {
            var categories = await _unitOfWork.Categories.GetWithProductsAsync();
            return _mapper.Map<IEnumerable<CategoryWithProductsDto>>(categories);
        }

        public async Task<bool> HasProductsAsync(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Invalid category ID", nameof(categoryId));

            return await _unitOfWork.Categories.HasProductsAsync(categoryId);
        }

        public async Task<bool> UpdateAsync(CategoryUpdateDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto));

            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(categoryDto.Id);
            if (existingCategory == null)
                  return false;

            _mapper.Map(categoryDto, existingCategory);
            existingCategory.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Categories.Update(existingCategory);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // Yangi qo'shilgan metodlar
        public async Task<bool> ExistsAsync(int id)
        {
            if (id <= 0) return false;
            return await _unitOfWork.Categories.ExistsAsync(id);
        }

        public async Task<CategoryDto> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty", nameof(name));

            var category = await _unitOfWork.Categories.GetByNameAsync(name);
            if (category == null)
                throw new NotFoundException("Category not found");

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
