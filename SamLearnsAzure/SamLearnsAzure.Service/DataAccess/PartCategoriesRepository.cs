using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Models;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.EFCore;

namespace SamLearnsAzure.Service.DataAccess
{
    public class PartCategoriesRepository : IPartCategoriesRepository
    {
        private readonly SamsAppDBContext _context;

        public PartCategoriesRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PartCategories>> GetPartCategories()
        {
            List<PartCategories> result = await _context.PartCategories
                 .OrderBy(p => p.Name)
                 .ToListAsync();
            return result;
        }
    }
}
