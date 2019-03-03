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

        public ColorsController(IColorsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetColors")]
        public async Task<IEnumerable<Colors>> GetColors()
        {
            return await _repo.GetColors();
        }

    }
}