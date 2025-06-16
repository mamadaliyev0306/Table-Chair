using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface ICartItemRepository:IGenericRepository<CartItem>
    {
        Task<CartItem> GetByIdCartItemAsync(int userId, int productId);
        void DeleteRangeAsync(IEnumerable<CartItem> items);
    }
}
