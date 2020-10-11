using Microsoft.AspNetCore.Mvc;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrowseThemesController : ControllerBase
    {
        private readonly IBrowseThemesRepository _repo;
        private readonly IRedisService _redisService;

        public BrowseThemesController(IBrowseThemesRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        /// <summary>
        /// Return a list of all BrowseThemes
        /// </summary>
        /// <param name="useCache">an optional parameter to use the Redis cache or now - used for troubleshooting, it is not recommended to edit this</param>
        /// <param name="year">an optional parameter to filter by year</param>
        /// <returns>an IEnumerable list of BrowseThemes objects</returns>
        [HttpGet("GetBrowseThemes")]
        public async Task<IEnumerable<BrowseThemes>> GetBrowseThemes(bool useCache = true, int? year = null)
        {
            return await _repo.GetBrowseThemes(_redisService, useCache, year);
        }

    }
}