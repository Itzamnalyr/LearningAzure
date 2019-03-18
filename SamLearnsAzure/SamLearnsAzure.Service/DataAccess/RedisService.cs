using System.Threading.Tasks;
using StackExchange.Redis;
using System;
using System.Diagnostics;

namespace SamLearnsAzure.Service.DataAccess
{

    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public RedisService(IDatabase database)
        {
            _database = database;
        }

        public async Task<string> GetAsync(string key)
        {
            string result = null;
            if (_database.IsConnected(key) == true)
            {
                result = await _database.StringGetAsync(key);
                if (string.IsNullOrEmpty(result) == false)
                {
                    Debug.WriteLine("Getting item from cache: " + key);
                }
            }
            return result;
        }

        public async Task<bool> SetAsync(string key, string data, TimeSpan expirationTime)
        {
            if (_database.IsConnected(key) == true)
            {
                Debug.WriteLine("Setting item into cache: " + key);
                return await _database.StringSetAsync(key, data, expirationTime);
            }
            else
            {
                return false;
            }
        }
    }
}
