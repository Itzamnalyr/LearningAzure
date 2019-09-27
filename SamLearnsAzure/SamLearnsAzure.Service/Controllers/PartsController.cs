using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Models;
using Microsoft.Data.SqlClient;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartsRepository _repo;
        private readonly IRedisService _redisService;

        public PartsController(IPartsRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        /// <summary>
        /// Return a list of all parts
        /// </summary>
        /// <param name="useCache">an optional parameter to use the Redis cache or now - used for troubleshooting, it is not recommended to edit this</param>
        /// <returns>an IEnumerable list of parts objects</returns>
        [HttpGet("GetParts")]
        public async Task<IEnumerable<Parts>> GetParts(bool useCache = true)
        {
            return await _repo.GetParts(_redisService, useCache);
        }

    }
}