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
    public class BrowseThemesRepository : BaseDataAccess<BrowseThemes>, IBrowseThemesRepository
    {
        private readonly IConfiguration _configuration;

        public BrowseThemesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }
     
        public async Task<IEnumerable<BrowseThemes>> GetBrowseThemes(IRedisService redisService, bool useCache, int? year)
        {
            string cacheKeyName = "BrowseThemes-" + year;
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);
            IEnumerable<BrowseThemes> result;

            //Check the cache
            string? cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<List<BrowseThemes>>(cachedJSON);
            }
            else
            {
                DynamicParameters parameters = new DynamicParameters();
                if (year != null)
                {
                    parameters.Add("@Year", year, DbType.Int32);
                }
                result = await base.GetList("GetBrowseThemes", parameters);
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
            return result ?? new List<BrowseThemes>();
        }
    }
}
