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
    public class ThemesController : ControllerBase
    {
        private readonly IThemesRepository _repo;

        public ThemesController(IThemesRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetThemes")]
        public async Task<IEnumerable<Themes>> GetThemes()
        {
            return await _repo.GetThemes();
        }
        
    }
}