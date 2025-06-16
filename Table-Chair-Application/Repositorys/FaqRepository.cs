using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class FaqRepository : GenericRepository<Faq>, IFaqRepository
    {
        public FaqRepository(FurnitureDbContext context) : base(context)
        {
        }
        // AnyAsync metodini yozish
        public async Task<bool> AnyAsync(IQueryable<Faq> faqs)
        {
            return await faqs.AnyAsync();
        }

        // ToListAsync metodini yozish
        public async Task<List<Faq>> ToListAsync(IQueryable<Faq> query)
        {
            return await query.ToListAsync();
        }
    }
}
