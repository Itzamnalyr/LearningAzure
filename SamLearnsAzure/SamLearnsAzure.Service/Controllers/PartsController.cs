using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.Models;
using System.Data.SqlClient;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartsRepository _repo;

        public PartsController(IPartsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetParts")]
        public async Task<IEnumerable<Parts>> GetParts()
        {
            return await _repo.GetParts();
        }

        //[HttpGet("GetPartsSummary")]
        //public async Task<IEnumerable<PartsSummary>> GetPartsSummary(string setNum)
        //{
        //    return await _repo.GetPartsSummary(setNum);
        //}

    }
}