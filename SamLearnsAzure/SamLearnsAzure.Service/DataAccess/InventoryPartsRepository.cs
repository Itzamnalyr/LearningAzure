using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Models;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.EFCore;
using Newtonsoft.Json;

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
            List<InventoryParts> result = await _context.InventoryParts.Take(1000)
                 .OrderBy(p => p.InventoryPartId)
                 .ToListAsync();

            return result;
        }
    }
}
