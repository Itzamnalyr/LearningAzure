﻿using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Storage.Blob;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace SamLearnsAzure.BingImageSearch.App
{
    class Program
    {
        private static string setName = "75955-1";

        static async Task Main(string[] args)
        {
            //Get configuration values from the appsettings.json file
            IConfiguration config = new ConfigurationBuilder()
                  .AddJsonFile("appsettings.json", true, true)
                  .Build();
            string storageConnectionString = config["storageConnectionString"];
            string cognitiveServicesSubscriptionKey = config["cognitiveServicesSubscriptionKey"];
            string cognitiveServicesUriBase = config["cognitiveServicesUriBase"];
            string storageContainerName = config["storageContainerName"];

            //1. Get image from Bing Image Search API
            SearchResult result = BingImageSearch(cognitiveServicesSubscriptionKey, cognitiveServicesUriBase, setName);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result.jsonResult);

            dynamic firstJsonObj = jsonObj["value"][0];
            string title = firstJsonObj["name"];
            string webUrl = firstJsonObj["webSearchUrl"];
            string imageUrl = (string)jsonObj.SelectToken("value[0].contentUrl");
            Console.WriteLine("Title for the first image result: " + title + "\n");
            Console.WriteLine("Web Url for the first image result: " + webUrl + "\n");
            Console.WriteLine("Image Url for the first image result: " + imageUrl + "\n");

            //2. Save image into blob storage
            string fileName = GetFileNameFromURL(imageUrl);
            fileName = ConvertFileNameToSetNumber(setName, fileName);
            bool saveResult = await SaveImageIntoBlob(storageConnectionString, storageContainerName, imageUrl, fileName);
            Console.WriteLine("Image saved into blob successfully: " + saveResult + "\n");

            //3. Validate save was successful
            bool imageExistsInBlob = CheckIfImageExistsInBlob(storageConnectionString, storageContainerName, fileName);
            if (imageExistsInBlob == true)
            {
                Console.WriteLine("Image successful saved into blob: " + setName + "\n");
            }
            else
            {
                Console.WriteLine("Poop. This didn't work: " + setName + "\n");
            }

        }

        static string ConvertFileNameToSetNumber(string setNum, string fileName)
        {
            string extension = Path.GetExtension(fileName);
            return setNum + extension;

        }

        static bool CheckIfImageExistsInBlob(string storageConnectionString, string storageContainerName, string searchTerm)
        {
            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlockBlob blob = cloudBlobClient.GetContainerReference(storageContainerName).GetBlockBlobReference(searchTerm);

                return blob.Exists();
            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
                return false;
            }
        }

        //Load up to the storage account, adapted from the Azure quick start for blob storage: 
        //https://github.com/Azure-Samples/storage-blobs-dotnet-quickstart/blob/master/storage-blobs-dotnet-quickstart/Program.cs
        static async Task<bool> SaveImageIntoBlob(string storageConnectionString, string containerName, string imageUrl, string fileName)
        {
            //Download the image
            byte[] fileBytes = await DownloadFile(imageUrl);

            CloudBlobContainer cloudBlobContainer;
            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a new container  
                    cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                    bool containerExists = cloudBlobContainer == null || await cloudBlobContainer.ExistsAsync();
                    if (containerExists == false)
                    {
                        await cloudBlobContainer.CreateAsync();
                        Console.WriteLine("Created container '{0}'", cloudBlobContainer.Name);
                    }
                    // Set the permissions so the blobs are container. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Get a reference to the blob address, then upload the file to the blob.
                    if (fileBytes.Length > 0)
                    {
                        using (MemoryStream stream = new MemoryStream(fileBytes, writable: false))
                        {
                            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                            await cloudBlockBlob.UploadFromStreamAsync(stream);
                        }
                    }
                    else
                    {
                        Console.WriteLine("File '" + imageUrl + "' is empty...");
                    }
                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }
                return true;
            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
                return false;
            }
        }

        private static string GetFileNameFromURL(string url)
        {
            string fileName = "";
            Uri uri = new Uri(url);
            fileName = System.IO.Path.GetFileName(uri.LocalPath);
            return fileName;
        }

        private static async Task<byte[]> DownloadFile(string url)
        {
            //retry the download 5 times 
            for (int retries = 0; retries < 5; retries++)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage result = await client.GetAsync(url))
                        {
                            if (result.IsSuccessStatusCode)
                            {
                                return await result.Content.ReadAsByteArrayAsync();
                            }
                        }
                    }
                }
                catch
                {
                    //do nothing, try again!
                    Console.WriteLine("Failed download - let's try again... (" + (retries + 1).ToString() + "/5");
                }
            }

            return null;
        }


        static SearchResult BingImageSearch(string cognitiveServicesSubscriptionKey, string cognitiveServicesUriBase, string searchTerm)
        {

            string uriQuery = cognitiveServicesUriBase + "?q=" + Uri.EscapeDataString(searchTerm) + "&safeSearch=strict";

            WebRequest request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = cognitiveServicesSubscriptionKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

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

        struct SearchResult
        {
            public string jsonResult;
            public Dictionary<string, string> relevantHeaders;
        }
    }
}