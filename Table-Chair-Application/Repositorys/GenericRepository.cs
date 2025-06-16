using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;

namespace Table_Chair_Application.Repositorys
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly FurnitureDbContext _context;

        public GenericRepository(FurnitureDbContext context)
        {
            _context = context;
        }

        // Add
        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        // Hard Delete
        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        //SofDelete
        public Task SoftDeleteAsync(TEntity entity)
        {
            var property = entity.GetType().GetProperty("IsDeleted");
            if (property != null && property.PropertyType == typeof(bool))
            {
                property.SetValue(entity, true);
                Update(entity);
            }
            else
            {
                throw new InvalidOperationException("Entity does not support soft delete.");
            }

            return Task.CompletedTask;
        }
        //Restore
        public Task RestoreAsync(TEntity entity)
        {
            var property = entity.GetType().GetProperty("IsDeleted");
            if (property != null && property.PropertyType == typeof(bool))
            {
                property.SetValue(entity, false);
                Update(entity);
            }
            else
            {
                throw new InvalidOperationException("Entity does not support restore operation.");
            }

            return Task.CompletedTask;
        }


        // Update
        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        // Get All (AsQueryable)
        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        // Get All Async (List)
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        // Get By Id
        public async Task<TEntity?> GetByIdAsync(int? id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        // Get Active (IsDeleted=false)
        public IQueryable<TEntity> GetActive()
        {
            var property = typeof(TEntity).GetProperty("IsDeleted");
            if (property != null)
            {
                return _context.Set<TEntity>().Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }
            return _context.Set<TEntity>();
        }

        // GetQueryable (Filter, Sort, Include)
        public IQueryable<TEntity> GetQueryable(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query;
        }

        // Pagination
        public async Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Set<TEntity>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Filtering + Sorting + Paging (PaginatedList return)
        public async Task<PaginatedList<TEntity>> GetFilteredSortedPagedAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);
            else
                query = query.OrderBy(e => true); // no specific ordering

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<TEntity>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = items
            };
        }

        // Count
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (filter != null)
                return await _context.Set<TEntity>().CountAsync(filter);
            return await _context.Set<TEntity>().CountAsync();
        }

        // Exists
        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate);
        }
    }
}

