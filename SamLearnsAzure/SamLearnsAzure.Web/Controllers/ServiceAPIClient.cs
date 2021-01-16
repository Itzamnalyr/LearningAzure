using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SamLearnsAzure.Models;

namespace SamLearnsAzure.Web.Controllers
{
    public class ServiceApiClient : IServiceApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public ServiceApiClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_configuration["AppSettings:WebServiceURL"])
            };
        }

        public async Task<List<Owners>> GetOwners()
        {
            Uri url = new Uri($"api/Owners/GetOwners", UriKind.Relative);
            List<Owners> results = await ReadMessageList<Owners>(url);
            if (results == null)
            {
                return new List<Owners>();
            }
            else
            {
                return results;
            }
        }

        public async Task<List<OwnerSets>> GetOwnerSets(int ownerId)
        {
            Uri url = new Uri($"api/OwnerSets/GetOwnerSets?ownerid=" + ownerId, UriKind.Relative);
            List<OwnerSets> results = await ReadMessageList<OwnerSets>(url);
            if (results == null)
            {
                return new List<OwnerSets>();
            }
            else
            {
                return results;
            }
        }

        public async Task<List<Sets>> GetSets()
        {
            Uri url = new Uri($"api/Sets/GetSets", UriKind.Relative);
            List<Sets> results = await ReadMessageList<Sets>(url);
            if (results == null)
            {
                return new List<Sets>();
            }
            else
            {
                return results;
            }
        }

        public async Task<Sets> GetSet(string setNum)
        {
            Uri url = new Uri($"api/Sets/GetSet?setnum=" + setNum, UriKind.Relative);
            Sets results = await ReadMessageItem<Sets>(url);
            if (results == null)
            {
                return new Sets();
            }
            else
            {
                return results;
            }
        }

        public async Task<SetImages> GetSetImage(string setNum)
        {
            Uri url = new Uri($"api/SetImages/GetSetImage?setnum=" + setNum + "&useCache=false", UriKind.Relative);
            SetImages results = await ReadMessageItem<SetImages>(url);
            if (results == null)
            {
                return new SetImages();
            }
            else
            {
                return results;
            }
        }

        public async Task<List<SetImages>> GetSetImages(string setNum, int resultsToReturn, int resultsToSearch)
        {
            Uri url = new Uri($"api/SetImages/GetSetImages?setnum=" + setNum + "&useCache=false&forceBingSearch=true&resultsToReturn=" + resultsToReturn + "&resultsToSearch=" + resultsToSearch, UriKind.Relative);
            List<SetImages> results = await ReadMessageList<SetImages>(url);
            if (results == null)
            {
                return new List<SetImages>();
            }
            else
            {
                return results;
            }
        }

        public async Task<SetImages> SaveSetImage(string setNum, string imageUrl)
        {
            Uri url = new Uri($"api/SetImages/SaveSetImage?setnum=" + setNum + "&imageUrl=" + HttpUtility.UrlEncode(imageUrl), UriKind.Relative);
            SetImages results = await ReadMessageItem<SetImages>(url);
            if (results == null)
            {
                return new SetImages();
            }
            else
            {
                return results;
            }
        }

        public async Task<List<SetParts>> GetSetParts(string setNum)
        {
            Uri url = new Uri($"api/SetParts/GetSetParts?setNum=" + setNum, UriKind.Relative);
            List<SetParts> results = await ReadMessageList<SetParts>(url);
            if (results == null)
            {
                return new List<SetParts>();
            }
            else
            {
                return results;
            }
        }

        public async Task<bool> SearchForMissingParts(string setNum)
        {
            Uri url = new Uri($"api/SetParts/SearchForMissingParts?setNum=" + setNum, UriKind.Relative);
            return await ReadMessageItem<bool>(url);
        }

        public async Task<List<Themes>> GetThemes()
        {
            Uri url = new Uri($"api/Themes/GetThemes", UriKind.Relative);
            return await ReadMessageList<Themes>(url);
        }

        public async Task<List<PartImages>> GetPartImages()
        {
            Uri url = new Uri($"api/PartImages/GetPartImages?useCache=false", UriKind.Relative);
            return await ReadMessageList<PartImages>(url);
        }

        public async Task<List<PartImages>> SearchForPotentialPartImages(string partNum, int colorId, string colorName, int resultsToReturn = 1, int resultsToSearch = 1)
        {
            Uri url = new Uri($"api/PartImages/SearchForPotentialPartImages?partNum=" + partNum + "&colorId=" + colorId + "&colorName=" + colorName + "&resultsToReturn=" + resultsToReturn + "&resultsToSearch=" + resultsToSearch, UriKind.Relative);
            return await ReadMessageList<PartImages>(url);
        }

        public async Task<List<BrowseThemes>> GetBrowseThemes(int? year)
        {
            Uri url = new Uri($"api/BrowseThemes/GetBrowseThemes?year=" + year, UriKind.Relative);
            return await ReadMessageList<BrowseThemes>(url);
        }

        public async Task<List<BrowseYears>> GetBrowseYears(int? themeId)
        {
            Uri url = new Uri($"api/BrowseYears/GetBrowseYears?themeId=" + themeId, UriKind.Relative);
            return await ReadMessageList<BrowseYears>(url);
        }

        public async Task<List<BrowseSets>> GetBrowseSets(int? themeId, int? year)
        {
            Uri url = new Uri($"api/BrowseSets/GetBrowseSets?themeId=" + themeId + "&year=" + year, UriKind.Relative);
            return await ReadMessageList<BrowseSets>(url);
        }

        private async Task<List<T>> ReadMessageList<T>(Uri url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode == true)
            {
                string bodyContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<T>>(bodyContent);
            }
            else
            {
                response.EnsureSuccessStatusCode();
                //Handle when the url is missing, preventing a 400 error.
#pragma warning disable CS8603 // Possible null reference return.
                return default;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        private async Task<T> ReadMessageItem<T>(Uri url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode == true)
            {
                string bodyContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(bodyContent);
            }
            else
            {
                response.EnsureSuccessStatusCode();
                //Handle when the url is missing, preventing a 400 error.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
                return default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
    }
}
