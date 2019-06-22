using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SamLearnsAzure.Service.AI
{
    public class ImageAnalysis
    {
        public async  Task<bool> PerformImageAnalysisSearch(string cognitiveServicesSubscriptionKey, string cognitiveServicesBingSearchUriBase, string imageUrl, string searchTerm)
        {
            HttpClient client = new HttpClient();
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cognitiveServicesSubscriptionKey);

            // Request parameters
            queryString["visualFeatures"] = "Tags";
            queryString["language"] = "en";
            string uri = cognitiveServicesBingSearchUriBase + queryString;

            // Request body
            // in the format: {"url":"http://example.com/images/test.jpg"}
            string jsonString = JsonConvert.SerializeObject(new
            {
                url = imageUrl
            });
            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Post request
            HttpResponseMessage response = await client.PostAsync(uri, content);
            string contentString = await response.Content.ReadAsStringAsync();
            client.Dispose();

            // Process response
            JObject joResponse = JObject.Parse(contentString);
            Console.WriteLine("\nResponse:\n\n{0}\n", joResponse.ToString());
            JArray array = (JArray)joResponse["tags"];
            bool foundSearchTerm = false;
            foreach (dynamic item in array)
            {
                //Search for the term - note: exact matches only!
                if (item.name.ToString().ToLower() == searchTerm)
                {
                    foundSearchTerm = true;
                    break;
                }

                Console.WriteLine("Tag: {0}, Confidence: {1}\n", item.name.ToString(), item.confidence.ToString("0.00%"));
            }

            return foundSearchTerm;
        }
    }
}
