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
    public class FeatureFlagsServiceApiClient : IFeatureFlagsServiceApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public FeatureFlagsServiceApiClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_configuration["AppSettings:FeatureFlagsWebServiceURL"])
            };
        }

        public async Task<bool> CheckFeatureFlag(string name, string environment)
        {
            Uri url = new Uri($"api/FeatureFlags/CheckFeatureFlag?name=" + name + "&environment=" + environment, UriKind.Relative);
            return await ReadMessageItem(url);
        }

        private async Task<bool> ReadMessageItem(Uri url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode == false)
            {
                return (bool)false;
            }
            else
            {
                return await response.Content.ReadAsAsync<bool>();
            }
        }
    }
}
