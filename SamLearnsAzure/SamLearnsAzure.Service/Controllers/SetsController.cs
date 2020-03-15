using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetsController : ControllerBase
    {
        private readonly ISetsRepository _repo;
        private readonly IRedisService _redisService;

        public SetsController(ISetsRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        /// <summary>
        /// Return a list of all sets
        /// </summary>
        /// <returns>an IEnumerable list of sets objects</returns>
        [HttpGet("GetSets")]
        public async Task<IEnumerable<Sets>> GetSets()
        {
            return await _repo.GetSets();
        }

        /// <summary>
        /// Return a list of all sets by theme id
        /// </summary>
        /// <returns>an IEnumerable list of sets objects</returns>
        [HttpGet("GetSetsByTheme")]
        public async Task<IEnumerable<Sets>> GetSetsByTheme(int themeId)
        {
            return await _repo.GetSetsByTheme(themeId);
        }

        /// <summary>
        /// Return a single set object
        /// </summary>
        /// <param name="setNum">a string set number, for example "75218-1"</param>
        /// <param name="useCache">an optional parameter to use the Redis cache or now - used for troubleshooting, it is not recommended to edit this</param>
        /// <returns>an single set object</returns>
        [HttpGet("GetSet")]
        public async Task<Sets> GetSet(string setNum, bool useCache = true)
        {
            return await _repo.GetSet(_redisService, useCache, setNum);
        }

    }
}