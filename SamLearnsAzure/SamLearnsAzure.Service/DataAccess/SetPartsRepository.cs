using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Models;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.EFCore;
using Newtonsoft.Json;

namespace SamLearnsAzure.Service.DataAccess
{
    public class SetPartsRepository : ISetPartsRepository
    {
        private readonly SamsAppDBContext _context;

        public SetPartsRepository(SamsAppDBContext context)
        {
            _context = context;
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
                result = JsonConvert.DeserializeObject<IEnumerable<SetParts>>(cachedJSON);
            }
            else
            {
                SqlParameter setNumParameter = new SqlParameter("SetNum", setNum);

                result = await _context
                    .Set<SetParts>()
                    .FromSqlRaw("EXECUTE dbo.GetSetParts @SetNum={0}", setNumParameter)
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



    }
}
