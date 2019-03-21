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

        /// <summary>
        /// Return a list of all inventories
        /// </summary>
        /// <param name="useCache">an optional parameter to use the Redis cache or now - used for troubleshooting, it is not recommended to edit this</param>
        /// <returns>an IEnumerable list of inventories objects</returns>
        [HttpGet("GetInventories")]
        public async Task<IEnumerable<Inventories>> GetInventories(bool useCache = true)
        {
            return await _repo.GetInventories(_redisService, useCache);
        }
        
    }
}