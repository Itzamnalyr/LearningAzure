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
    public class SetsRepository : ISetsRepository
    {
        private readonly SamsAppDBContext _context;

        public SetsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sets>> GetSets(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "Sets-all";
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);
            List<Sets> result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = JsonConvert.DeserializeObject<List<Sets>>(cachedJSON);
            }
            else
            {
                result = await _context.Sets
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                if (redisService != null)
                {
                    //set the cache with the updated record
                    await redisService.SetAsync(cacheKeyName, JsonConvert.SerializeObject(result), cacheExpirationTime);
                }
            }

            return result;
        }

        public async Task<IEnumerable<SetParts>> GetSetParts(IRedisService redisService, bool useCache, string setNum)
        {
            string cacheKeyName = "SetParts-" + setNum;
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);
            IEnumerable<SetParts> result;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = JsonConvert.DeserializeObject<IEnumerable<SetParts>>(cachedJSON);
            }
            else
            {
                SqlParameter setNumParameter = new SqlParameter("SetNum", setNum);

                result = await _context
                    .Query<SetParts>()
                    .FromSql("EXECUTE dbo.GetSetParts @SetNum", setNumParameter)
                    .ToListAsync();

                if (redisService != null)
                {
                    //set the cache with the updated record
                    await redisService.SetAsync(cacheKeyName, JsonConvert.SerializeObject(result), cacheExpirationTime);
                }
            }

            return result;
        }

        public async Task<Sets> GetSet(IRedisService redisService, bool useCache, string setNum)
        {
            string cacheKeyName = "Set-" + setNum;
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);
            Sets result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = JsonConvert.DeserializeObject<Sets>(cachedJSON);
            }
            else
            {
                result = await _context.Sets
                    .Include(t => t.Theme)
                    .SingleAsync(b => b.SetNum == setNum);

                if (redisService != null)
                {
                    //set the cache with the updated record
                    await redisService.SetAsync(cacheKeyName, JsonConvert.SerializeObject(result), cacheExpirationTime);
                }
            }

            return result;
        }
    }
}
