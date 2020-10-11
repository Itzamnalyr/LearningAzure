using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface IBrowseYearsRepository
    {
        Task<IEnumerable<BrowseYears>> GetBrowseYears(IRedisService redisService, bool useCache, int? themeId);
    }
}
