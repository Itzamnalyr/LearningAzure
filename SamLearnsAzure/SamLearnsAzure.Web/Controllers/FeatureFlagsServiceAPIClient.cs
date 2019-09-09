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
            return await ReadMessageItem<bool>(url);
        }

        private async Task<T> ReadMessageItem<T>(Uri url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            return await response.Content.ReadAsAsync<T>();
        }
    }
}
