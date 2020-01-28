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
    public class SetsRepository : ISetsRepository
    {
        private readonly SamsAppDBContext _context;

        public SetsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sets>> GetSets()
        {
            List<Sets> result = await _context.Sets
                .OrderBy(p => p.Name)
                .ToListAsync();

            return result;
        }

        public async Task<Sets> GetSet(IRedisService redisService, bool useCache, string setNum)
        {
            string cacheKeyName = "Set-" + setNum;
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);
            Sets result;

            //Check the cache
            string? cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<Sets>(cachedJSON);
            }
            else
            {
                result = await _context.Sets
                    .Include(t => t.Theme)
                    .SingleOrDefaultAsync(b => b.SetNum == setNum);

                if (result != null && redisService != null)
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

            return result ?? new Sets();
        }

    }
}
