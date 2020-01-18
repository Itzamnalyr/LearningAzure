using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Storage.Blob;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using System.Text;
using System.Runtime.InteropServices;
using System.Web;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace SamLearnsAzure.BingImageSearch.App
{
    class Program
    {
        private static IConfiguration _configuration;

        static async Task Main()
        {
            string setNum = "75218-1";

            //0. Get configuration values from the appsettings.json file
            _configuration = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json", true, true)
                      .Build();

            //1. Get images from Bing Image Search API
            List<SetImages> setImages = await GetSetImages(setNum, 10);

            //2. Validate that the image is a set, and not a false positive
            SetImages validateSetImage = null;
            foreach (SetImages item in setImages)
            {
                if (await TestImage(item.SetImage) == true)
                {
                    validateSetImage = item;
                    break;
                }
            }

            //3. Validate image was successfully found
            if (validateSetImage != null)
            {
                Console.WriteLine("Image successful found!! \n");
            }
            else
            {
                Console.WriteLine("Poop. This didn't work: \n");
            }

        }

        private async static Task<bool> TestImage(string imageUrl)
        {
            string cognitiveServicesSubscriptionKey = _configuration["CognitiveServicesSubscriptionKey"];
            string cognitiveServicesImageAnalysisUriBase = _configuration["cognitiveServicesImageAnalysisUriBase"];
            string searchTerm = "lego";

            return await ImageAnalysisSearch(cognitiveServicesSubscriptionKey, cognitiveServicesImageAnalysisUriBase, imageUrl, searchTerm);

        }

        private async static Task<List<SetImages>> GetSetImages(string setNum, int resultsToReturn = 1)
        {
            List<SetImages> setImages = new List<SetImages>();

            //1. We don't need the image from the repo/redis

            //2. Get image from Bing Image Search API
            string cognitiveServicesSubscriptionKey = _configuration["CognitiveServicesSubscriptionKey"];
            string cognitiveServicesBingSearchUriBase = _configuration["CognitiveServicesBingSearchUriBase"];
            SearchResult result = await BingImageSearch(cognitiveServicesSubscriptionKey, cognitiveServicesBingSearchUriBase, setNum, resultsToReturn);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result.jsonResult);

            for (int i = 0; i < resultsToReturn; i++)
            {
                dynamic firstJsonObj = jsonObj["value"][i];
                string title = firstJsonObj["name"];
                string webUrl = firstJsonObj["webSearchUrl"];
                string imageUrl = (string)jsonObj.SelectToken("value[" + i + "].contentUrl");
                Console.WriteLine("Title for the " + i + " image result: " + title + "\n");
                Console.WriteLine("Web Url for the " + i + " image result: " + webUrl + "\n");
                Console.WriteLine("Image Url for the " + i + " image result: " + imageUrl + "\n");
                SetImages newSetImage = new SetImages
                {
                    SetNum = setNum,
                    SetImage = imageUrl
                };
                setImages.Add(newSetImage);
            }

            return setImages;
        }

        private async static Task<SearchResult> BingImageSearch(string cognitiveServicesSubscriptionKey, string cognitiveServicesBingSearchUriBase, string searchTerm, int resultsToReturn)
        {

            string uriQuery = cognitiveServicesBingSearchUriBase + "?q=" + Uri.EscapeDataString(searchTerm) + "&safeSearch=strict&count=" + resultsToReturn.ToString();

            WebRequest request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = cognitiveServicesSubscriptionKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            string json = await streamReader.ReadToEndAsync();
            streamReader.Dispose();

            // Create the result object for return
            SearchResult searchResult = new SearchResult()
            {
                jsonResult = json,
                relevantHeaders = new Dictionary<string, string>()
            };

            // Extract Bing HTTP headers
            foreach (string header in response.Headers)
            {
                if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                {
                    searchResult.relevantHeaders[header] = response.Headers[header];
                }
            }
            return searchResult;
        }

        private async static Task<bool> ImageAnalysisSearch(string cognitiveServicesSubscriptionKey, string cognitiveServicesImageAnalysisUriBase, string imageUrl, string searchTerm)
        {
            HttpClient client = new HttpClient();
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cognitiveServicesSubscriptionKey);

            // Request parameters
            queryString["visualFeatures"] = "Tags";
            queryString["language"] = "en";
            string uri = cognitiveServicesImageAnalysisUriBase + queryString;

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

        struct SearchResult
        {
            public string jsonResult;
            public Dictionary<string, string> relevantHeaders;
        }

    }
}
