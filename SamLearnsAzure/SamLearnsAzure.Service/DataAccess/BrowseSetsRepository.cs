using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.Dapper;

namespace SamLearnsAzure.Service.DataAccess
{
    public class BrowseSetsRepository : BaseDataAccess<BrowseSets>, IBrowseSetsRepository
    {
        private readonly IConfiguration _configuration;

        public BrowseSetsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<BrowseSets>> GetBrowseSets(IRedisService redisService, bool useCache, int? themeId, int? year)
        {
            string cacheKeyName = "BrowseSets-" + themeId + "-" + year;
            TimeSpan cacheExpirationTime = new TimeSpan(0, 5, 0);
            IEnumerable<BrowseSets> result;

            //Check the cache
            string? cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<List<BrowseSets>>(cachedJSON);
            }
            else
            {
                DynamicParameters parameters = new DynamicParameters();
                if (themeId != null)
                {
                    parameters.Add("@ThemeId", themeId, DbType.Int32);
                }
                if (year != null)
                {
                    parameters.Add("@Year", year, DbType.Int32);
                }
                result = await base.GetList("GetBrowseSets", parameters);
                if (result != null && redisService != null)
                {
                    //set the cache with the updated record
                    string json = JsonConvert.SerializeObject(result, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    //Only save to REDIS if the length of the json is less than 100KB, a REDIS best practice
                    if (json.Length < 100000)
                    {
                        await redisService.SetAsync(cacheKeyName, json, cacheExpirationTime);
                    }
                }
            }
            return result ?? new List<BrowseSets>();
        }

    }
}
