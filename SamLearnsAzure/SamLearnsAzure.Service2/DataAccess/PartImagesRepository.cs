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
    public class PartImagesRepository : BaseDataAccess<PartImages>, IPartImagesRepository
    {
        private readonly IConfiguration _configuration;

        public PartImagesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<PartImages>> GetPartImages(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "PartImages";
            TimeSpan cacheExpirationTime = new TimeSpan(0, 30, 0);
            IEnumerable<PartImages> result;

            //Check the cache
            string? cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<List<PartImages>>(cachedJSON);
            }
            else
            {
                result = await base.GetList("GetPartImages");
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
            return result ?? new List<PartImages>();
        }

        public async Task<PartImages> GetPartImage(IRedisService redisService, bool useCache, string partNum)
        {
            string cacheKeyName = "PartImage-" + partNum;
            TimeSpan cacheExpirationTime = new TimeSpan(0, 30, 0);
            PartImages result;

            //Check the cache
            string? cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<PartImages>(cachedJSON);
            }
            else
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PartNum", partNum, DbType.String);
                result = await base.GetItem("GetPartImages", parameters);
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
            return result ?? new PartImages();
        }

        public async Task<PartImages> SavePartImage(IRedisService redisService, PartImages partImage)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PartNum", partImage.PartNum, DbType.String);
            parameters.Add("@SourceImage", partImage.SourceImage, DbType.String);
            parameters.Add("@ColorId", partImage.ColorId, DbType.Int32);
            await base.SaveItem("SavePartImage", parameters);
            return await GetPartImage(redisService, false, partImage.PartNum);
        }
    }
}
