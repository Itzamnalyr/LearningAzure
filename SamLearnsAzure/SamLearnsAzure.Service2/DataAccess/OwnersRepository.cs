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
    public class OwnersRepository : BaseDataAccess<Owners>, IOwnersRepository
    {
        private readonly IConfiguration _configuration;

        public OwnersRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<Owners>> GetOwners(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "Owners-all";
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);
            IEnumerable<Owners> result;

            //Check the cache
            string? cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<List<Owners>>(cachedJSON);
            }
            else
            {
                result = await base.GetList("GetOwners");
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
            return result ?? new List<Owners>();
        }

        public async Task<Owners> GetOwner(IRedisService redisService, bool useCache, int ownerId)
        {
            string cacheKeyName = "Owners-" + ownerId;
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);

            Owners result;

            //Check the cache
            string? cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<Owners>(cachedJSON);
            }
            else
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@OwnerId", ownerId, DbType.Int32);
                result = await base.GetItem("GetOwners", parameters);
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
            return result ?? new Owners();
        }
    }
}
