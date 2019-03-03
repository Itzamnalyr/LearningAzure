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
    public class InventoryPartsController : ControllerBase
    {
        private readonly IInventoryPartsRepository _repo;

        public InventoryPartsController(IInventoryPartsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetInventoryParts")]
        public async Task<IEnumerable<InventoryParts>> GetInventoryParts()
        {
            return await _repo.GetInventoryParts();
        }
        
    }
}