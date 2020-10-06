using System;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface IRedisService
    {
        Task<string?> GetAsync(string key);
        Task<bool> SetAsync(string key, string data, TimeSpan expirationTime);
    }
}
