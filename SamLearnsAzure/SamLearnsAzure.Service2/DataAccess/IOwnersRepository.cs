using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface IOwnersRepository
    {
        Task<IEnumerable<Owners>> GetOwners(IRedisService redisService, bool useCache);

        Task<Owners> GetOwner(IRedisService redisService, bool useCache, int ownerId);
    }
}
