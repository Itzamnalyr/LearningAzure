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
    public class PartRelationshipsController : ControllerBase
    {
        private readonly IPartRelationshipsRepository _repo;
        private readonly IRedisService _redisService;

        public PartRelationshipsController(IPartRelationshipsRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        [HttpGet("GetPartRelationships")]
        public async Task<IEnumerable<PartRelationships>> GetPartRelationships(bool useCache = true)
        {
            return await _repo.GetPartRelationships(_redisService, useCache);
        }
        
    }
}