using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.AI;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetImagesController : ControllerBase
    {
        private readonly ISetImagesRepository _repo;
        private readonly IRedisService _redisService;
        private readonly IConfiguration _configuration;

        public SetImagesController(ISetImagesRepository repo, IRedisService redisService, IConfiguration configuration)
        {
            _repo = repo;
            _redisService = redisService;
            _configuration = configuration;
        }

        [HttpGet("GetSetImages")]
        public async Task<List<SetImages>> GetSetImages(string setNum, int resultsToReturn = 1, int resultsToSearch = 1)
        {
            string tagFilter = "lego";
            string cognitiveServicesSubscriptionKey = _configuration["CognitiveServicesSubscriptionKey"]; // The subscription key is coming from key vault
            string cognitiveServicesBingSearchUriBase = _configuration["AppSettings:CognitiveServicesBingSearchUriBase"];
            string cognitiveServicesImageAnalysisUriBase = _configuration["AppSettings:CognitiveServicesImageAnalysisUriBase"];

            //1. Get image from Bing Image Search API
            BingImageSearch bingImageSearchAI = new BingImageSearch();
            List<SetImages> setImages = await bingImageSearchAI.PerformBingImageSearch(cognitiveServicesSubscriptionKey,
                cognitiveServicesBingSearchUriBase, cognitiveServicesImageAnalysisUriBase,
                setNum, resultsToReturn, resultsToSearch, tagFilter);

            return setImages;
        }

        [HttpGet("GetSetImage")]
        public async Task<SetImages> GetSetImage(string setNum, bool useCache = true, bool forceBingSearch = false, int resultsToReturn = 1, int resultsToSearch = 10)
        {
            //1. Service looks in database to see if image exists?
            SetImages setImage = await _repo.GetSetImage(_redisService, useCache, setNum);

            if (setImage == null || forceBingSearch == true)
            {
                string tagFilter = "lego";
                string cognitiveServicesSubscriptionKey = _configuration["CognitiveServicesSubscriptionKey"]; // The subscription key is coming from key vault
                string cognitiveServicesBingSearchUriBase = _configuration["AppSettings:CognitiveServicesBingSearchUriBase"];
                string cognitiveServicesImageAnalysisUriBase = _configuration["AppSettings:CognitiveServicesImageAnalysisUriBase"];

                //1. Get image from Bing Image Search API
                BingImageSearch bingImageSearchAI = new BingImageSearch();
                List<SetImages> setImages = await bingImageSearchAI.PerformBingImageSearch(cognitiveServicesSubscriptionKey,
                    cognitiveServicesBingSearchUriBase, cognitiveServicesImageAnalysisUriBase,
                    setNum, resultsToReturn, resultsToSearch, tagFilter);

                //2. Save image into blob storage
                if (setImages.Count > 0)
                {
                    setImage = await SaveSetImage(setImages[0], setNum, setImages[0].SetImage);
                }
            }

            //3. Service returns Image
            return setImage;
        }

        [HttpGet("SaveSetImage")]
        public async Task<SetImages> SaveSetImage(string setNum, string imageUrl)
        {
            //Update database with new image, but don't force it if there is a setimage already
            return await SaveSetImage(null, setNum, imageUrl);
        }

        private async Task<SetImages> SaveSetImage(SetImages setImage, string setNum, string imageUrl)
        {
            //Get the storage blob connection information
            string storageContainerName = _configuration["AppSettings:StorageContainerName"];
            string storageConnectionString = _configuration["AppSettings:StorageConnectionString"];
            storageConnectionString = storageConnectionString.Replace("[ACCOUNTNAME]", _configuration["AppSettings:StorageAccountName"]);
            storageConnectionString = storageConnectionString.Replace("[ACCOUNTKEY]", _configuration["StorageAccountKey" + _configuration["AppSettings:Environment"]]);

            //extract the filename from the image url
            string fileName = GetFileNameFromURL(imageUrl);
            fileName = ConvertFileNameToSetNumber(setNum, fileName);

            //Save the image to the storage blob
            bool saveResult = await SaveImageIntoBlob(storageConnectionString, storageContainerName, imageUrl, fileName);
            Console.WriteLine("Image saved into blob successfully: " + saveResult + "\n");

            if (setImage == null)
            {
                setImage = new SetImages
                {
                    SetNum = setNum,
                    SetImage = fileName
                };
                setImage = await _repo.SaveSetImage(setImage);
            }
            return setImage;
        }

        private string ConvertFileNameToSetNumber(string setNum, string fileName)
        {
            string extension = Path.GetExtension(fileName);
            return setNum + extension;
        }

        //Load up to the storage account, adapted from the Azure quick start for blob storage: 
        //https://github.com/Azure-Samples/storage-blobs-dotnet-quickstart/blob/master/storage-blobs-dotnet-quickstart/Program.cs
        private async Task<bool> SaveImageIntoBlob(string storageConnectionString, string containerName, string imageUrl, string fileName)
        {
            bool result = false;
            //Download the image
            byte[] fileBytes = await DownloadFile(imageUrl);

            CloudBlobContainer cloudBlobContainer;
            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
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

                result = true;
            }
            return result;
        }

        private string GetFileNameFromURL(string url)
        {
            Uri uri = new Uri(url);
            return System.IO.Path.GetFileName(uri.LocalPath);
        }

        private async Task<byte[]> DownloadFile(string url)
        {
            byte[] file = null;
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
                                file = await result.Content.ReadAsByteArrayAsync();
                                break;
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

            return file;
        }





    }
}