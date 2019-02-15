using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.Models;

namespace SamLearnsAzure.Service.DataAccess
{
    public class ColorsRepository : IColorsRepository
    {
        private readonly SamsAppDBContext _context;

        public ColorsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Colors>> GetColors()
        {
            List<Colors> result = await _context.Colors
                 .OrderBy(p => p.Name)
                 .ToListAsync();
            return result;
        }
    }
}
