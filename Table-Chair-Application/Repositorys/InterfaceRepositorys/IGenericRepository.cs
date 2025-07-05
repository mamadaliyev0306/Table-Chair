using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        IQueryable<TEntity> GetAll();
        Task<TEntity?> GetByIdAsync(int? id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task RestoreAsync(TEntity entity);
        Task SoftDeleteAsync(TEntity entity);
        IQueryable<TEntity> GetActive();
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize);
        IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");
        Task<PaginatedList<TEntity>> GetFilteredSortedPagedAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int pageNumber = 1,
        int pageSize = 10);

        Task<TEntity?> GetByIdIncludingDeletedAsync(int id);
    }
}
