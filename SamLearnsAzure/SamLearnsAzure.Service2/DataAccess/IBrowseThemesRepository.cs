using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface IBrowseThemesRepository
    {
        Task<IEnumerable<BrowseThemes>> GetBrowseThemes(IRedisService redisService, bool useCache, int? year);
    }
}
