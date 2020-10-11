using Microsoft.AspNetCore.Mvc;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrowseSetsController : ControllerBase
    {
        private readonly IBrowseSetsRepository _repo;
        private readonly IRedisService _redisService;

        public BrowseSetsController(IBrowseSetsRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        /// <summary>
        /// Return a list of all BrowseSets
        /// </summary>
        /// <param name="useCache">an optional parameter to use the Redis cache or now - used for troubleshooting, it is not recommended to edit this</param>
        /// <param name="themeId">an optional parameter to filter by theme</param>
        /// <param name="year">an optional parameter to filter by year</param>
        /// <returns>an IEnumerable list of BrowseSets objects</returns>
        [HttpGet("GetBrowseSets")]
        public async Task<IEnumerable<BrowseSets>> GetBrowseSets(bool useCache = true, int? themeId = null, int? year = null)
        {
            return await _repo.GetBrowseSets(_redisService, useCache, themeId, year);
        }

    }
}