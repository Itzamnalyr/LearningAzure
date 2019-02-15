using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace SamLearnsAzure.Service.DataAccess
{
    public class OwnerSetsRepository : IOwnerSetsRepository
    {
        private readonly SamsAppDBContext _context;

        public OwnerSetsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OwnerSets>> GetOwnerSets()
        {
            List<OwnerSets> result = await _context.OwnerSets
                .Include(l => l.Set)
                    .ThenInclude(t => t.Theme)
                .OrderBy(p => p.OwnerId)
                .ToListAsync();
            return result;
        }
    }
}
