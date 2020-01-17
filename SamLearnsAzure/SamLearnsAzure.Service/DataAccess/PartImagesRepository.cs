using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Models;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.EFCore;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using System.Collections;

namespace SamLearnsAzure.Service.DataAccess
{
    public class PartImagesRepository : IPartImagesRepository
    {
        private readonly SamsAppDBContext _context;

        public PartImagesRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<List<PartImages>> GetPartImages(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "PartImages";
            TimeSpan cacheExpirationTime = new TimeSpan(0, 30, 0);
            List<PartImages> result = null;

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
                result = await _context.PartImages
                    //.Include(t => t.Color)
                    .OrderBy(p => p.PartNum)
                    .ToListAsync();

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

            return result;
        }

        public async Task<PartImages> GetPartImage(IRedisService redisService, bool useCache, string partNum)
        {
            string cacheKeyName = "PartImage-" + partNum;
            TimeSpan cacheExpirationTime = new TimeSpan(0, 30, 0);
            PartImages result = null;

            //Check the cache
            string cachedJSON = null;
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
                result = await _context.PartImages
                    .FirstOrDefaultAsync(b => b.PartNum == partNum);

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
            SqlParameter partNumParameter = new SqlParameter("@PartNum", partImage.PartNum);
            SqlParameter partImageParameter = new SqlParameter("@SourceImage", partImage.SourceImage);
            SqlParameter colorIdParameter = new SqlParameter("@ColorId", partImage.ColorId);

            await _context.Database.ExecuteSqlRawAsync("dbo.SavePartImage @PartNum={0}, @SourceImage={1}, @ColorId={2}", partNumParameter, partImageParameter, colorIdParameter);

            return await GetPartImage(redisService, false, partImage.PartNum);
        }
    }
}
