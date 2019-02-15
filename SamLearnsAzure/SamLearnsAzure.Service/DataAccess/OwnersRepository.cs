using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public class OwnersRepository : IOwnersRepository
    {
        private readonly SamsAppDBContext _context;

        public OwnersRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Owners>> GetOwners()
        {
            List<Owners> result = await _context.Owners
                 .OrderBy(p => p.OwnerName)
                 .ToListAsync();
            return result;
        }

        public async Task<Owners> GetOwner(int ownerId)
        {
            Owners result = await _context.Owners
                .Where(p => p.Id == ownerId)
                .FirstOrDefaultAsync<Owners>();
            return result;
        }
    }
}
