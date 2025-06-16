using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IFaqService
    {
        Task<IEnumerable<FaqDto>> SearchAsync(string searchTerm);
        Task<IEnumerable<FaqDto>> GetAllAsync(
              Expression<Func<Faq, bool>>? filter = null,
              Func<IQueryable<Faq>, IOrderedQueryable<Faq>>? orderBy = null,
              int? page = null,
              int? pageSize = null);
        Task<FaqDto> GetByIdAsync(int id);
        Task CreateAsync(FaqCreateDto faqCreateDto);
        Task UpdateAsync(int id,FaqUpdateDto faqDto);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<Faq, bool>> filter);
        Task<int> CountAsync(Expression<Func<Faq, bool>>? filter = null);
        Task<IEnumerable<FaqDto>> GetActiveAsync();
        Task<IEnumerable<FaqDto>> DynamicFilterAsync(Dictionary<string, object> filters, int? page = null, int? pageSize = null);
    }
}
