using Microsoft.AspNetCore.Mvc;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoriesRepository _repo;

        public InventoriesController(IInventoriesRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetInventories")]
        public async Task<IEnumerable<Inventories>> GetInventories()
        {
            return await _repo.GetInventories();
        }
        
    }
}