using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.Dapper;

namespace SamLearnsAzure.Service.DataAccess
{
    public class InventorySetsRepository : BaseDataAccess<InventorySets>, IInventorySetsRepository
    {
        private readonly IConfiguration _configuration;

        public InventorySetsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<InventorySets>> GetInventorySets()
        {
            IEnumerable<InventorySets> result = await base.GetList("GetInventorySets");
            return result;
        }
    }
}
