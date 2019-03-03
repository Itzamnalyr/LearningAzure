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
    public class PartRelationshipsRepository : IPartRelationshipsRepository
    {
        private readonly SamsAppDBContext _context;

        public PartRelationshipsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PartRelationships>> GetPartRelationships()
        {
            List<PartRelationships> result = await _context.PartRelationships
                 .OrderBy(p => p.PartRelationshipId)
                 .ToListAsync();
            return result;
        }
    }
}
