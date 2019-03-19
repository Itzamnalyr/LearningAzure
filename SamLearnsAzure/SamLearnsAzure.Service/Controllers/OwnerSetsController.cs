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

        [HttpGet("GetOwnerSets")]
        public async Task<IEnumerable<OwnerSets>> GetOwnerSets(int ownerId, bool useCache = true)
        {
            return await _repo.GetOwnerSets(_redisService, useCache, ownerId);
        }
        
    }
}