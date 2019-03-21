using Microsoft.AspNetCore.Mvc;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly IColorsRepository _repo;
        private readonly IRedisService _redisService;

        public ColorsController(IColorsRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        /// <summary>
        /// Return a list of all colors
        /// </summary>
        /// <param name="useCache">an optional parameter to use the Redis cache or now - used for troubleshooting, it is not recommended to edit this</param>
        /// <returns>an IEnumerable list of colors objects</returns>
        [HttpGet("GetColors")]
        public async Task<IEnumerable<Colors>> GetColors(bool useCache = true)
        {
            return await _repo.GetColors(_redisService, useCache);
        }

    }
}