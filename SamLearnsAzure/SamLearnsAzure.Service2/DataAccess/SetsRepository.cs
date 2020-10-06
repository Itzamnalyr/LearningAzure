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
    public class SetsRepository : BaseDataAccess<Sets>, ISetsRepository
    {
        private readonly IConfiguration _configuration;

        public SetsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<Sets>> GetSets()
        {
            IEnumerable<Sets> result = await base.GetList("GetSets");
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
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@SetNum", setNum, DbType.String);
                result = await base.GetItem("GetSets", parameters);
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
