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
    public class PartsRepository : IPartsRepository
    {
        private readonly SamsAppDBContext _context;

        public PartsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Parts>> GetParts()
        {
            List<Parts> result = await _context.Parts
                 .OrderBy(p => p.Name)
                 .ToListAsync();
            return result;
        }
       
    }
}
