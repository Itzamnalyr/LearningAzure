using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Service.EFCore;
using System;
using Newtonsoft.Json;

namespace SamLearnsAzure.Service.DataAccess
{
    public class OwnersRepository : IOwnersRepository
    {
        private readonly SamsAppDBContext _context;

        public OwnersRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Owners>> GetOwners(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "Owners-all";
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);
            List<Owners> result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = JsonConvert.DeserializeObject<List<Owners>>(cachedJSON);
            }
            else
            {
                result = await _context.Owners
                  .OrderBy(p => p.OwnerName)
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

        public async Task<Owners> GetOwner(IRedisService redisService, bool useCache, int ownerId)
        {
            string cacheKeyName = "Owners-" + ownerId;
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);

            Owners result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = JsonConvert.DeserializeObject<Owners>(cachedJSON);
            }
            else
            {
                result = await _context.Owners
                .Where(p => p.Id == ownerId)
                .FirstOrDefaultAsync<Owners>();

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
