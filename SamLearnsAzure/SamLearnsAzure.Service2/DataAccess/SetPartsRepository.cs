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
    public class SetPartsRepository : BaseDataAccess<SetParts>, ISetPartsRepository
    {
        private readonly IConfiguration _configuration;

        public SetPartsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<SetParts>> GetSetParts(IRedisService redisService, bool useCache, string setNum)
        {
            string cacheKeyName = "SetParts-" + setNum;
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);
            IEnumerable<SetParts> result;

            //Check the cache
            string? cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<List<SetParts>>(cachedJSON);
            }
            else
            {
                if (string.IsNullOrEmpty(setNum) == false)
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@SetNum", setNum, DbType.String);
                    result = await base.GetList("GetSetParts", parameters);
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
                else
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@SetNum", setNum, DbType.String);
                    result = await base.GetList("GetSetParts", parameters);
                }
            }
            return result ?? new List<SetParts>();
        }

    }
}