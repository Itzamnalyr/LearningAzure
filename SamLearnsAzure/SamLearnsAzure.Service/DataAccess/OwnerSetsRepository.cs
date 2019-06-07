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
            if (redisService != null && useCache)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = JsonConvert.DeserializeObject<List<OwnerSets>>(cachedJSON);
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

                if (redisService != null)
                {
                    //set the cache with the updated record
                    string json = JsonConvert.SerializeObject(result, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    //Only save to REDIS is the length of the json is less than 100KB, a REDIS best practice
                    if (json.Length < 100000)
                    {
                        await redisService.SetAsync(cacheKeyName, json, cacheExpirationTime);
                    }
                }
            }

            return result;
        }
    }
}
