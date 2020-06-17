using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.Dapper;

namespace SamLearnsAzure.Service.DataAccess
{
    public class InventoriesRepository : BaseDataAccess<Inventories>, IInventoriesRepository
    {
        private readonly IConfiguration _configuration;

        public InventoriesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<Inventories>> GetInventories(IRedisService redisService, bool useCache)
        {
            IEnumerable<Inventories> result = await base.GetList("GetInventories");
            return result;
        }
    }
}
