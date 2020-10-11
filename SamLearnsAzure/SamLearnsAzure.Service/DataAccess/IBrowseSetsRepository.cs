using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface IBrowseSetsRepository
    {
        Task<IEnumerable<BrowseSets>> GetBrowseSets(IRedisService redisService, bool useCache, int? themeId, int? year);
    }
}
