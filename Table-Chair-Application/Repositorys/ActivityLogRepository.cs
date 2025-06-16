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
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly FurnitureDbContext _context;

        public ActivityLogRepository(FurnitureDbContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetByUserIdAsync(int userId) =>
              _context.Users
            .Where(a => a.Equals(userId))
            .AsQueryable();
    }
}
