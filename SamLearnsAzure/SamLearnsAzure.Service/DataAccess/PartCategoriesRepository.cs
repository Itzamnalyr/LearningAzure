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
    public class PartCategoriesRepository : IPartCategoriesRepository
    {
        private readonly SamsAppDBContext _context;

        public PartCategoriesRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PartCategories>> GetPartCategories(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "PartCategories-all";
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);

            List<PartCategories> result;

            //Check the cache
            string? cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<List<PartCategories>>(cachedJSON);
            }
            else
            {
                result = await _context.PartCategories
                 .OrderBy(p => p.Name)
                 .ToListAsync();

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

            return result ?? new List<PartCategories>();
        }
    }
}
