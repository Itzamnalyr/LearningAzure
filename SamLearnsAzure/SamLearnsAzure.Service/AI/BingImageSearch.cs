using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.AI
{
    public class BingImageSearch
    {
        public async Task<SearchResult> PerformBingImageSearch(string cognitiveServicesSubscriptionKey, string cognitiveServicesBingSearchUriBase, string searchTerm, int resultsToReturn)
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

        public struct SearchResult
        {
            public string jsonResult;
            public Dictionary<string, string> relevantHeaders;
        }
    }
}
