using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.Dapper;

namespace SamLearnsAzure.Service.DataAccess
{
    public class PartRelationshipsRepository : BaseDataAccess<PartRelationships>, IPartRelationshipsRepository
    {
        private readonly IConfiguration _configuration;

        public PartRelationshipsRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            base.SetupConnectionString(_configuration);
        }

        public async Task<IEnumerable<PartRelationships>> GetPartRelationships()
        {
            IEnumerable<PartRelationships> result = await base.GetList("GetPartRelationships");
            return result;
        }
    }
}
