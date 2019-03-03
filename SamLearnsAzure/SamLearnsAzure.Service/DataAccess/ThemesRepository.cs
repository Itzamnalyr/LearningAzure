using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Models;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.EFCore;

namespace SamLearnsAzure.Service.DataAccess
{
    public class ThemesRepository : IThemesRepository
    {
        private readonly SamsAppDBContext _context;

        public ThemesRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Themes>> GetThemes()
        {
            List<Themes> result = await _context.Themes
                 .OrderBy(p => p.Name)
                 .ToListAsync();
            return result;
        }
    }
}
