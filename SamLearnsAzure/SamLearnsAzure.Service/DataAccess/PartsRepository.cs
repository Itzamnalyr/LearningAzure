using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.Dapper;

namespace SamLearnsAzure.Service.DataAccess
{
    public class PartsRepository : BaseDataAccess<Parts>, IPartsRepository
    {
        private readonly IConfiguration _configuration;

        public PartsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<Parts>> GetParts(IRedisService redisService, bool useCache)
        {
            IEnumerable<Parts> result = await base.GetList("GetParts");

            return result;
        }

    }
}
