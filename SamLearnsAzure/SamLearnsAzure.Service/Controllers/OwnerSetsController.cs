using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerSetsController : ControllerBase
    {
        private readonly IOwnerSetsRepository _repo;
        private readonly IRedisService _redisService;

        public OwnerSetsController(IOwnerSetsRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        /// <summary>
        /// Return a list of all owner sets
        /// </summary>
        /// <param name="ownerId">a integer owner number, for example "1"</param>
        /// <param name="useCache">an optional parameter to use the Redis cache or now - used for troubleshooting, it is not recommended to edit this</param>
        /// <returns></returns>
        [HttpGet("GetOwnerSets")]
        public async Task<IEnumerable<OwnerSets>> GetOwnerSets(int ownerId, bool useCache = true)
        {
            return await _repo.GetOwnerSets(_redisService, useCache, ownerId);
        }
        
        [HttpGet("SaveOwnerSet")]
        public async Task<bool> SaveOwnerSet(int ownerId, string setNum)
        {
            return await _repo.SaveOwnerSet(ownerId, setNum);
        }

    }
}