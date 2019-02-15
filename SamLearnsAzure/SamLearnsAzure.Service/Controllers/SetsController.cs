using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.Models;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetsController : ControllerBase
    {
        private readonly ISetsRepository _repo;

        public SetsController(ISetsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetSets")]
        public async Task<IEnumerable<Sets>> GetSets()
        {
            return await _repo.GetSets();
        }
        
        //[HttpGet("GetSet")]
        //public async Task<Sets> GetSet(string setNum)
        //{
        //    return await _repo.GetSet(setNum);
        //}

    }
}