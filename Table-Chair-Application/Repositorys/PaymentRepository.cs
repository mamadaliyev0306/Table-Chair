using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly FurnitureDbContext _context;
        public PaymentRepository(FurnitureDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
               .Where(p => p.PaidAt >= startDate && p.PaidAt <= endDate)
               .OrderByDescending(p => p.PaidAt)
               .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId)
        {
            return await _context
                .Payments.Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.OrderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaidAt ?? p.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Payments.AnyAsync(p => p.Id == id);
        }
    }
}
