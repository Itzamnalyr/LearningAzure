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
        Task<List<OwnerSets>> GetOwnerSets(int ownerId);
        Task<Sets> GetSet(string setNum);
        Task<SetImages> GetSetImage(string setNum);
        Task<List<SetImages>> GetSetImages(string setNum, int resultsToReturn, int resultsToSearch);
        Task<SetImages> SaveSetImage(string setNum, string imageUrl);
        Task<List<SetParts>> GetSetParts(string setNum);
        Task<bool> SearchForMissingParts(string setNum);
        Task<List<PartImages>> GetPartImages();
        Task<List<PartImages>> SearchForPotentialPartImages(string partNum, int colorId, string colorName, int resultsToReturn = 1, int resultsToSearch = 1);
    }

}
