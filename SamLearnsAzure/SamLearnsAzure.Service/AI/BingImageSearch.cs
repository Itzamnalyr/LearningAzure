using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SamLearnsAzure.Models;

namespace SamLearnsAzure.Service.AI
{
    public class BingImageSearch
    {
        public async Task<List<BingSearchResult>> PerformBingImageSearch(string cognitiveServicesSubscriptionKey,
            string cognitiveServicesBingSearchUriBase, string cognitiveServicesImageAnalysisUriBase,
            string searchTerm, int resultsToReturn, int resultsToSearch, string tagFilter)
        {
            List<BingSearchResult> images = new List<BingSearchResult>();

            //create query and send request
            string uriQuery = cognitiveServicesBingSearchUriBase + "?q=" + Uri.EscapeDataString(searchTerm) + "&safeSearch=strict&count=" + resultsToSearch.ToString();
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
            foreach (string? header in response.Headers)
            {
                if (header != null)
                {
                    if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                    {
                        searchResult.relevantHeaders[header] = response.Headers[header];
                    }
                }
            }

            //Process the results
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(searchResult.jsonResult);
            for (int i = 0; i < jsonObj["value"].Count; i++)
            {
                dynamic firstJsonObj = jsonObj["value"][i];
                string title = firstJsonObj["name"];
                string webUrl = firstJsonObj["webSearchUrl"];
                string imageUrl = (string)jsonObj.SelectToken("value[" + i + "].contentUrl");
                Console.WriteLine("Title for the " + i + " image result: " + title + "\n");
                Console.WriteLine("Web Url for the " + i + " image result: " + webUrl + "\n");
                Console.WriteLine("Image Url for the " + i + " image result: " + imageUrl + "\n");

                //Make sure that the image contains the search term we are looking for
                if (tagFilter != null)
                {
                    ImageAnalysis imageAnalysisAI = new ImageAnalysis();
                    bool imageContainsSearchTerm = await imageAnalysisAI.PerformImageAnalysisSearch(cognitiveServicesSubscriptionKey, cognitiveServicesImageAnalysisUriBase, imageUrl, tagFilter);
                    if (imageContainsSearchTerm == true)
                    {
                        BingSearchResult newImage = new BingSearchResult
                        {
                            SearchTerm = searchTerm,
                            ImageUrl = imageUrl
                        };
                        images.Add(newImage);
                    }
                    if (images.Count >= resultsToReturn)
                    {
                        break;
                    }
                }
                else
                {
                    BingSearchResult newImage = new BingSearchResult
                    {
                        SearchTerm = searchTerm,
                        ImageUrl = imageUrl
                    };
                    images.Add(newImage);
                    break;
                }
            }

            return images;
        }

        public class BingSearchResult
        {
            public string? SearchTerm
            {
                get; set;
            }
            public string? ImageUrl
            {
                get; set;
            }
        }


        public struct SearchResult
        {
            public string jsonResult;
            public Dictionary<string, string> relevantHeaders;
        }
    }
}
