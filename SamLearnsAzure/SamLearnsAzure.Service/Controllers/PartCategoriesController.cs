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
    public class PartCategoriesController : ControllerBase
    {
        private readonly IPartCategoriesRepository _repo;
        private readonly IRedisService _redisService;

        public PartCategoriesController(IPartCategoriesRepository repo, IRedisService redisService)
        {
            _repo = repo;
            _redisService = redisService;
        }

        [HttpGet("GetPartCategories")]
        public async Task<IEnumerable<PartCategories>> GetPartCategories(bool useCache = true)
        {
            return await _repo.GetPartCategories(_redisService, useCache);
        }
        
    }
}