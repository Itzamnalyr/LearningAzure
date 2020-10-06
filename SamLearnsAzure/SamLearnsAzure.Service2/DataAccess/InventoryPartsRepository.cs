using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.Dapper;

namespace SamLearnsAzure.Service.DataAccess
{
    public class InventoryPartsRepository : BaseDataAccess<InventoryParts>, IInventoryPartsRepository
    {
        private readonly IConfiguration _configuration;

        public InventoryPartsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<InventoryParts>> GetInventoryParts(string partNum)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PartNum", partNum, DbType.String);
            IEnumerable<InventoryParts> result = await base.GetList("GetInventoryParts", parameters);
            return result;
        }
    }
}
