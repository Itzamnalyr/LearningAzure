using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Service.Models;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerSetsController : ControllerBase
    {
        private readonly IOwnerSetsRepository _repo;

        public OwnerSetsController(IOwnerSetsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetOwnerSets")]
        public async Task<IEnumerable<OwnerSets>> GetOwnerSets()
        {
            return await _repo.GetOwnerSets();
        }
        
    }
}