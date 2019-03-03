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
    public class SetsRepository : ISetsRepository
    {
        private readonly SamsAppDBContext _context;

        public SetsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sets>> GetSets()
        {
            List<Sets> result = await _context.Sets
                 .OrderBy(p => p.Name)
                 .ToListAsync();
            return result;
        }

        public async Task<IEnumerable<SetParts>> GetSetParts(string setNum)
        {
            SqlParameter setNumParameter = new SqlParameter("SetNum", setNum);

            IEnumerable<SetParts> result = await _context
               .Query<SetParts>()
               .FromSql("EXECUTE dbo.GetSetParts @SetNum", setNumParameter)
               .ToListAsync();

            return result;
        }

        public async Task<Sets> GetSet(string setNum)
        {
            Sets result = await _context.Sets
                .Include(t => t.Theme)
                .SingleAsync(b => b.SetNum == setNum);

            return result;
        }
    }
}
