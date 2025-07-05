using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(CreateProductDto product)
        {
            if (product == null)
                throw new BadRequestException("Mahsulot ma'lumotlari bo'sh bo'lishi mumkin emas.");

            try
            {
                var productMap = _mapper.Map<Product>(product);
                productMap.CreatedAt = DateTime.UtcNow;
                productMap.UpdatedAt = DateTime.UtcNow;
                productMap.IsDeleted = false;
                await _unitOfWork.Products.AddAsync(productMap);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Mahsulot qo'shishda xatolik: {ex.Message}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Id noto'g'ri.");

            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                    throw new NotFoundException($"Id: {id} bo'yicha mahsulot topilmadi.");

                product.IsDeleted = true; // Soft delete
                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Mahsulot o'chirishda xatolik: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                if (products == null || !products.Any())
                    throw new NotFoundException("Mahsulotlar mavjud emas.");
                var filtered = products.Where(p => !p.IsDeleted);
                return _mapper.Map<IEnumerable<ProductDto>>(filtered);
            }
            catch (Exception ex)
            {
                throw new Exception($"Mahsulotlarni olishda xatolik: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _unitOfWork.Products.GetByCategoryAsync(categoryId);
                var filtered = products.Where(p => !p.IsDeleted);
                return _mapper.Map<IEnumerable<ProductDto>>(filtered);
            }
            catch (Exception ex)
            {
                throw new Exception($"Kategoriya bo'yicha mahsulotlar olishda xatolik: {ex.Message}");
            }
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null || product.IsDeleted)
                    throw new NotFoundException($"Id: {id} bo'yicha mahsulot topilmadi.");

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                throw new Exception($"Id bo'yicha mahsulotni olishda xatolik: {ex.Message}");
            }
        }

        public IQueryable<ProductDto> GetFilteredProducts(ProductFilterDto filter)
        {
            try
            {
                var query = _unitOfWork.Products.GetFilteredProductsQuery(filter)
                    .Where(p => !p.IsDeleted)
                    .ProjectTo<ProductDto>(_mapper.ConfigurationProvider);

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception($"Filtrlab mahsulotlarni olishda xatolik: {ex.Message}");
            }
        }

        public IQueryable<ProductDto> SearchProduct(string searchTerm)
        {
            try
            {
                var query = _unitOfWork.Products.SearchAsync(searchTerm)
                    .Where(p => !p.IsDeleted)
                    .ProjectTo<ProductDto>(_mapper.ConfigurationProvider);

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception($"Mahsulotlarni qidirishda xatolik: {ex.Message}");
            }
        }

        public async Task UpdateAsync(UpdateProductDto productDto)
        {
            if (productDto == null)
                throw new BadRequestException("Mahsulot ma'lumotlari to'liq emas.");

            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productDto.Id);
                if (product == null || product.IsDeleted)
                    throw new NotFoundException($"Id: {productDto.Id} bo'yicha mahsulot topilmadi.");

                _mapper.Map(productDto, product); // Product ustiga update qiladi
                product.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Mahsulot yangilashda xatolik: {ex.Message}");
            }
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            if (quantity < 0)
                throw new BadRequestException("Miqdor manfiy bo'lishi mumkin emas.");

            try
            {
                await _unitOfWork.Products.UpdateStockAsync(productId, quantity);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Mahsulot stokini yangilashda xatolik: {ex.Message}");
            }
        }
        public async Task<PaginatedList<ProductDto>> GetFilteredProductsAsync(ProductFilterDto filterDto, int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.Products.GetFilteredProductsAsync(filterDto, pageNumber, pageSize);

            var resultDtoItems = _mapper.Map<List<ProductDto>>(result.Items);

            var resultDto = new PaginatedList<ProductDto>(
                resultDtoItems,
                result.TotalCount,
                result.PageNumber,
                result.PageSize
            );

            return resultDto;
        }

        public async Task SoftDeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Products.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new NotFoundException("Error");
                }
                await _unitOfWork.Products.SoftDeleteAsync(entity);
                entity.IsDeleted = true;
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error" + ex.Message);
            }


        }

        public async Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize)
        {
            // Filter faqat berilgan kategoriyaga tegishli va o'chirilmagan mahsulotlar
            Expression<Func<Product, bool>> filter = p =>
                p.CategoryId == categoryId &&
                !p.IsDeleted; // Fixed the null check issue by directly checking IsDeleted as a non-nullable bool

            // Sort by name (optional)
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = q => q.OrderBy(p => p.Name);

            // Generic repositorydagi pagination metodidan foydalanish
            var result = await _unitOfWork.Products.GetFilteredSortedPagedAsync(
                filter: filter,
                orderBy: orderBy,
                pageNumber: pageNumber,
                pageSize: pageSize
            );

            var mappedItems = _mapper.Map<List<ProductDto>>(result.Items);

            return new PaginatedList<ProductDto>(
                mappedItems,
                result.TotalCount,
                result.PageNumber,
                result.PageSize
            );
        }
        public async Task<List<ProductDto>> GetProductsWithWishlistInfoAsync(int userId)
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var wishlistProductIds = (await _unitOfWork.Wishlist.GetWishlistProductsAsync(userId))
                .Select(w => w.ProductId)
                .ToList(); // Extract ProductId into a list for Contains to work

            // AutoMapper bilan mapping + IsLiked ni qo‘lda to‘ldirish
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            foreach (var dto in productDtos)
            {
                dto.IsLiked = wishlistProductIds.Contains(dto.Id); // Now Contains will work correctly
            }

            return productDtos;
        }
    }
}


