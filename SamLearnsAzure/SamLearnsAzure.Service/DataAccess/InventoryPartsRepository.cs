using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace SamLearnsAzure.Service.DataAccess
{
    public class InventoryPartsRepository : IInventoryPartsRepository
    {
        private readonly SamsAppDBContext _context;

        public InventoryPartsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryParts>> GetInventoryParts()
        {
            List<InventoryParts> result = await _context.InventoryParts
                 .OrderBy(p => p.InventoryPartId)
                 .ToListAsync();
            return result;
        }
    }
}
