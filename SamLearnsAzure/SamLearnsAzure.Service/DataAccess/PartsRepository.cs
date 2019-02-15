using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Service.Models;
using Microsoft.EntityFrameworkCore;

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

        //public async Task<IEnumerable<PartsSummary>> GetPartsSummary(string setNum)
        //{
        //    SqlParameter setNumParameter = new SqlParameter("SetNum", setNum);

        //    IEnumerable<PartsSummary> result = await _context
        //       .Query<PartsSummary>()
        //       .FromSql("EXECUTE dbo.GetPartsSummary @SetNum", setNumParameter)
        //       .ToListAsync();

        //    return result;
        //}
    }
}
