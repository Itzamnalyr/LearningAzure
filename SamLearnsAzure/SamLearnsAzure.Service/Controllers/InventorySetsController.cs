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
    public class InventorySetsController : ControllerBase
    {
        private readonly IInventorySetsRepository _repo;
        private readonly IRedisService _redisService;

        public InventorySetsController(IInventorySetsRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        [HttpGet("GetInventorySets")]
        public async Task<IEnumerable<InventorySets>> GetInventorySets(bool useCache = true)
        {
            return await _repo.GetInventorySets(_redisService, useCache);
        }
        
    }
}