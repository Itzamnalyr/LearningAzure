using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface IOwnersRepository
    {
        Task<IEnumerable<Owners>> GetOwners();

        Task<Owners> GetOwner(int ownerId);
    }
}
