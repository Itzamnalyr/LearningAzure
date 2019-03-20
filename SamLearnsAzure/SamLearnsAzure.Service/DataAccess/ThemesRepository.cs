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
    public class ThemesRepository : IThemesRepository
    {
        private readonly SamsAppDBContext _context;

        public ThemesRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Themes>> GetThemes(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "Themes-all";
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);

            List<Themes> result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = JsonConvert.DeserializeObject<List<Themes>>(cachedJSON);
            }
            else
            {
                result = await _context.Themes
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
