using Microsoft.AspNetCore.Mvc;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnersRepository _repo;
        private readonly IRedisService _redisService;

        public OwnersController(IOwnersRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        [HttpGet("GetOwners")]
        public async Task<IEnumerable<Owners>> GetOwners(bool useCache = true)
        {
            return await _repo.GetOwners(_redisService, useCache);
        }

        [HttpGet("GetOwner")]
        public async Task<Owners> GetOwner(int ownerId, bool useCache = true)
        {
            return await _repo.GetOwner(_redisService, useCache, ownerId);
        }
    }
}