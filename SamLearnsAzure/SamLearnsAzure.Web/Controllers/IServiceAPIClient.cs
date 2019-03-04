using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Controllers
{

    public interface IServiceAPIClient
    {
        //Task<List<Owners>> GetOwners();
        Task<List<OwnerSets>> GetOwnerSets(int ownedId);
        //Task<List<Sets>> GetSets();
        Task<Sets> GetSet(string setNum);
        Task<List<SetParts>> GetSetParts(string setNum);
        //Task<List<Themes>> GetThemes();
    }

}
