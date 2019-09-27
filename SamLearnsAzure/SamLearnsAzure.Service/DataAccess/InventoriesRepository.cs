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
    public class InventoriesRepository : IInventoriesRepository
    {
        private readonly SamsAppDBContext _context;

        public InventoriesRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventories>> GetInventories(IRedisService redisService, bool useCache)
        {
            List<Inventories> result = await _context.Inventories
                 .OrderBy(p => p.Id)
                 .ToListAsync();

            return result;
        }
    }
}
