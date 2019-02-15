using Microsoft.AspNetCore.Mvc;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnersRepository _repo;

        public OwnersController(IOwnersRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetOwners")]
        public async Task<IEnumerable<Owners>> GetOwners()
        {
            return await _repo.GetOwners();
        }

        [HttpGet("GetOwner")]
        public async Task<Owners> GetOwner(int ownerId)
        {
            return await _repo.GetOwner(ownerId);
        }
    }
}