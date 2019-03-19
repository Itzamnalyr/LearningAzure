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

        [HttpGet("GetColors")]
        public async Task<IEnumerable<Colors>> GetColors(bool useCache = true)
        {
            return await _repo.GetColors(_redisService, useCache);
        }

    }
}