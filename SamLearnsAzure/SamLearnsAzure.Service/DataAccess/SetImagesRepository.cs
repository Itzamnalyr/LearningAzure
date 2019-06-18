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
    public class SetImagesRepository : ISetImagesRepository
    {
        private readonly SamsAppDBContext _context;

        public SetImagesRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<SetImages> GetSetImage(IRedisService redisService, bool useCache, string setNum)
        {
            string cacheKeyName = "SetImage-" + setNum;
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);
            SetImages result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null) //This will be null if we aren't using Redis or the item doesn't exist in Redis
            {
                result = JsonConvert.DeserializeObject<SetImages>(cachedJSON);
            }
            else
            {
                result = await _context.SetImages
                    .SingleOrDefaultAsync(b => b.SetNum == setNum);

                if (redisService != null)
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

        public async Task<SetImages> SaveSetImage(SetImages setImage)
        {
            await _context.SetImages.AddAsync(setImage);
            await _context.SaveChangesAsync();

            SetImages result = await _context.SetImages
                    .FirstOrDefaultAsync(b => b.SetNum == setImage.SetNum);

            return result;
        }
    }
}
