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
    public class OwnerSetsRepository : IOwnerSetsRepository
    {
        private readonly SamsAppDBContext _context;

        public OwnerSetsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OwnerSets>> GetOwnerSets(IRedisService redisService, bool useCache, int ownerId)
        {
            string cacheKeyName = "OwnerSets-" + ownerId;
            TimeSpan cacheExpirationTime = new TimeSpan(0, 5, 0);

            List<OwnerSets> result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OwnerSets>>(cachedJSON);
            }
            else
            {
                result = await _context.OwnerSets
                .Include(l => l.Set)
                    .ThenInclude(t => t.Theme)
                .Include(l => l.Owner)
                .Where(p => p.OwnerId == ownerId)
                .OrderBy(p => p.OwnerId)
                .ToListAsync();
            }

            return result;
        }
    }
}
