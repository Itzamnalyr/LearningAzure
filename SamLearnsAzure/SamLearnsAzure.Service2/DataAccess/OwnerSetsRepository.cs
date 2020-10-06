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
    public class OwnerSetsRepository : BaseDataAccess<OwnerSets>, IOwnerSetsRepository
    {
        private readonly IConfiguration _configuration;

        public OwnerSetsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<OwnerSets>> GetOwnerSets(IRedisService redisService, bool useCache, int ownerId)
        {
            string cacheKeyName = "OwnerSets-" + ownerId;
            TimeSpan cacheExpirationTime = new TimeSpan(0, 5, 0);

            IEnumerable<OwnerSets> result;

            //Check the cache
            string? cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<List<OwnerSets>>(cachedJSON);
            }
            else
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@OwnerId", ownerId, DbType.Int32);
                result = await base.GetList("GetOwnerSets", parameters);
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
            return result ?? new List<OwnerSets>();
        }

        public async Task<bool> SaveOwnerSet(string setNum, int ownerId, bool owned, bool wanted)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@SetNum", setNum, DbType.String);
            parameters.Add("@OwnerId", ownerId, DbType.Int32);
            parameters.Add("@Owned", owned, DbType.Boolean);
            parameters.Add("@Wanted", wanted, DbType.Boolean);
            return await base.SaveItem("SaveOwnerSet", parameters);
        }
    }
}
