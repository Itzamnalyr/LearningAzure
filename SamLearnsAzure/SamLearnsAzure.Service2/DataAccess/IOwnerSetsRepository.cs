using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface IOwnerSetsRepository
    {
        Task<IEnumerable<OwnerSets>> GetOwnerSets(IRedisService redisService, bool useCache, int ownerId);

        Task<bool> SaveOwnerSet(string setNum, int ownerId, bool owned, bool wanted);
    }
}
