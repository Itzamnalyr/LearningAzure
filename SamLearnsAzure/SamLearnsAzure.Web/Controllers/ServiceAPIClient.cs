using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

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
            return await ReadMessageList<Owners>(url);
        }

        public async Task<List<OwnerSets>> GetOwnerSets(int ownerId)
        {
            Uri url = new Uri($"api/OwnerSets/GetOwnerSets?ownerid=" + ownerId, UriKind.Relative);
            return await ReadMessageList<OwnerSets>(url);
        }

        public async Task<List<Sets>> GetSets()
        {
            Uri url = new Uri($"api/Sets/GetSets", UriKind.Relative);
            return await ReadMessageList<Sets>(url);
        }

        public async Task<Sets> GetSet(string setNum)
        {
            Uri url = new Uri($"api/Sets/GetSet?setnum=" + setNum, UriKind.Relative);
            return await ReadMessageItem<Sets>(url);
        }

        public async Task<SetImages> GetSetImage(string setNum)
        {
            Uri url = new Uri($"api/SetImages/GetSetImage?setnum=" + setNum + "&useCache=false", UriKind.Relative);
            return await ReadMessageItem<SetImages>(url);
        }

        public async Task<List<SetImages>> GetSetImages(string setNum)
        {
            Uri url = new Uri($"api/SetImages/GetSetImages?setnum=" + setNum + "&useCache=false&forceBingSearch=true&resultsToReturn=10&resultsToSearch=20", UriKind.Relative);
            return await ReadMessageList<SetImages>(url);
        }

        public async Task<SetImages> SaveSetImage(string setNum, string imageUrl)
        {
            Uri url = new Uri($"api/SetImages/SaveSetImage?setnum=" + setNum + "&imageUrl=" + HttpUtility.UrlEncode(imageUrl), UriKind.Relative);
            return await ReadMessageItem<SetImages>(url);
        }

        public async Task<List<SetParts>> GetSetParts(string setNum)
        {
            Uri url = new Uri($"api/SetParts/GetSetParts?setNum=" + setNum, UriKind.Relative);
            return await ReadMessageList<SetParts>(url);
        }

        public async Task<bool> RefreshSetParts(string setNum)
        {
            Uri url = new Uri($"api/SetParts/RefreshSetParts?setNum=" + setNum, UriKind.Relative);
            return await ReadMessageItem<bool>(url);
        }

        public async Task<List<Themes>> GetThemes()
        {
            Uri url = new Uri($"api/Themes/GetThemes", UriKind.Relative);
            return await ReadMessageList<Themes>(url);
        }

        private async Task<List<T>> ReadMessageList<T>(Uri url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            return await response.Content.ReadAsAsync<List<T>>();
        }

        private async Task<T> ReadMessageItem<T>(Uri url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            return await response.Content.ReadAsAsync<T>();
        }
    }
}
