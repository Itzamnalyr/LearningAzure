using Microsoft.AspNetCore.Mvc;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrowseYearsController : ControllerBase
    {
        private readonly IBrowseYearsRepository _repo;
        private readonly IRedisService _redisService;

        public BrowseYearsController(IBrowseYearsRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        /// <summary>
        /// Return a list of all BrowseYears
        /// </summary>
        /// <param name="useCache">an optional parameter to use the Redis cache or now - used for troubleshooting, it is not recommended to edit this</param>
        /// <param name="themeId">an optional parameter to filter by theme</param>
        /// <returns>an IEnumerable list of BrowseYears objects</returns>
        [HttpGet("GetBrowseYears")]
        public async Task<IEnumerable<BrowseYears>> GetBrowseYears(bool useCache = true, int? themeId = null)
        {
            return await _repo.GetBrowseYears(_redisService, useCache, themeId);
        }

    }
}