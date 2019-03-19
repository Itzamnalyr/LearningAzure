using Microsoft.AspNetCore.Mvc;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoriesRepository _repo;
        private readonly IRedisService _redisService;

        public InventoriesController(IInventoriesRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        [HttpGet("GetInventories")]
        public async Task<IEnumerable<Inventories>> GetInventories(bool useCache = true)
        {
            return await _repo.GetInventories(_redisService, useCache);
        }
        
    }
}