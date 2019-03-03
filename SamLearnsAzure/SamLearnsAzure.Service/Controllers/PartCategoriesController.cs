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

        public PartCategoriesController(IPartCategoriesRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetPartCategories")]
        public async Task<IEnumerable<PartCategories>> GetPartCategories()
        {
            return await _repo.GetPartCategories();
        }
        
    }
}