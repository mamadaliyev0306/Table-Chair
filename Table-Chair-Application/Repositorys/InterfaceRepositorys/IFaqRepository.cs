using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface IFaqRepository : IGenericRepository<Faq>
    {
        Task<bool> AnyAsync(IQueryable<Faq> faqs);
        Task<List<Faq>> ToListAsync(IQueryable<Faq> query);
    }
}
