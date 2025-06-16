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
    public class AboutInfoRepository : GenericRepository<AboutInfo>, IAboutInfoRepository
    {
        public AboutInfoRepository(FurnitureDbContext context) : base(context)
        {
        }
    }
}
