using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace SamLearnsAzure.Service.DataAccess
{
    public class InventorySetsRepository : IInventorySetsRepository
    {
        private readonly SamsAppDBContext _context;

        public InventorySetsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventorySets>> GetInventorySets()
        {
            List<InventorySets> result = await _context.InventorySets
                 .OrderBy(p => p.InventoryId)
                 .ToListAsync();
            return result;
        }
    }
}
