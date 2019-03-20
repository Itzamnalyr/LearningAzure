using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.EFCore;

namespace SamLearnsAzure.Service.DataAccess
{
    public class ColorsRepository : IColorsRepository
    {
        private readonly SamsAppDBContext _context;

        public ColorsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Colors>> GetColors(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "Colors-all";
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);
            List<Colors> result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = JsonConvert.DeserializeObject<List<Colors>>(cachedJSON);
            }
            else
            {
                result = await _context.Colors
                .OrderBy(p => p.Name)
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
