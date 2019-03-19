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

            List<PartCategories> result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PartCategories>>(cachedJSON);
            }
            else
            {
                result = await _context.PartCategories
                 .OrderBy(p => p.Name)
                 .ToListAsync();
            }

            return result;
        }
    }
}
