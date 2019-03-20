using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public async Task<IEnumerable<InventoryParts>> GetInventoryParts(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "InventoryParts-all";
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);

            List<InventoryParts> result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = JsonConvert.DeserializeObject<List<InventoryParts>>(cachedJSON);
            }
            else
            {
                result = await _context.InventoryParts.Take(1000)
                 .OrderBy(p => p.InventoryPartId)
                 .ToListAsync();

                if (redisService != null)
                {
                    //set the cache with the updated record
                    await redisService.SetAsync(cacheKeyName, JsonConvert.SerializeObject(result), cacheExpirationTime);
                }
            }
            return result;
        }
    }
}
