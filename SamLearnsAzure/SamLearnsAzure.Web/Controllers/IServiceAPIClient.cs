using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SamLearnsAzure.Web.Controllers
{

    public interface IServiceApiClient
    {
        //Task<List<Owners>> GetOwners();
        Task<List<OwnerSets>> GetOwnerSets(int ownerId);
        //Task<List<Sets>> GetSets();
        Task<Sets> GetSet(string setNum);
        Task<SetImages> GetSetImage(string setNum);
        Task<List<SetImages>> GetSetImages(string setNum);
        Task<SetImages> SaveSetImage(string setNum, string imageUrl);
        Task<List<SetParts>> GetSetParts(string setNum);
        Task<bool> RefreshSetParts(string setNum);
        //Task<List<Themes>> GetThemes();
    }

}
