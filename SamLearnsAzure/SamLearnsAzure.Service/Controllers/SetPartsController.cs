using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.AI;
using SamLearnsAzure.Service.DataAccess;
using static SamLearnsAzure.Service.AI.BingImageSearch;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetPartsController : ControllerBase
    {
        private readonly ISetPartsRepository _repo;
        private readonly IPartImagesRepository _repoPartImages;
        private readonly IRedisService _redisService;
        private readonly IConfiguration _configuration;

        public SetPartsController(ISetPartsRepository repo, IPartImagesRepository repoPartImages, IRedisService redisService, IConfiguration configuration)
        {
            _repo = repo;
            _repoPartImages = repoPartImages;
            _redisService = redisService;
            _configuration = configuration;
        }

        /// <summary>
        /// Return a list of all set parts
        /// </summary>
        /// <param name="setNum">a string set number, for example "75218-1"</param>
        /// <param name="useCache">an optional parameter to use the Redis cache or now - used for troubleshooting, it is not recommended to edit this</param>
        /// <returns>an IEnumerable list of set parts objects</returns>
        [HttpGet("GetSetParts")]
        public async Task<IEnumerable<SetParts>> GetSetParts(string setNum, bool useCache = true)
        {
            return await _repo.GetSetParts(_redisService, useCache, setNum);
        }

        /// <summary>
        /// Search for missing parts in a set, checking that the parts exist in storage, and downloading the parts if needed.
        /// </summary>
        /// <param name="setNum"></param>
        /// <returns></returns>
        [HttpGet("SearchForMissingParts")]
        public async Task<bool> SearchForMissingParts(string setNum)
        {
            //Get a list of all parts for this set
            IEnumerable<SetParts> setParts = await _repo.GetSetParts(_redisService, false, setNum);

            //Read the storage information from the key vault
            string storageContainerPartImagesName = _configuration["AppSettings:StorageContainerPartImagesName"];
            string storageConnectionString = _configuration["AppSettings:StorageConnectionString"];
            storageConnectionString = storageConnectionString.Replace("[ACCOUNTNAME]", _configuration["AppSettings:StorageAccountName"]);
            storageConnectionString = storageConnectionString.Replace("[ACCOUNTKEY]", _configuration["StorageAccountKey" + _configuration["AppSettings:Environment"]]);

            //Loop through each set part, and check if the blob exists
            foreach (SetParts item in setParts)
            {
                string newImageName = item.ColorId + "/" + item.PartNum + ".png";
                if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
                {
                    //Check if the image exists in storage
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    bool blobExists = await cloudBlobClient.GetContainerReference(storageContainerPartImagesName)
                           .GetBlockBlobReference(newImageName)
                           .ExistsAsync();

                    if (blobExists == false)
                    {
                        //Process the image
                        BingImageSearch bingImageSearchAPI = new BingImageSearch();
                        string cognitiveServicesSubscriptionKey = _configuration["CognitiveServicesSubscriptionKey"]; // The subscription key is coming from key vault
                        string cognitiveServicesBingSearchUriBase = _configuration["AppSettings:CognitiveServicesBingSearchUriBase"];
                        string cognitiveServicesImageAnalysisUriBase = _configuration["AppSettings:CognitiveServicesImageAnalysisUriBase"];
                        string tagFilter = "lego";
                        string searchTerm = item.PartNum + " lego " + item.ColorName;

                        //Search Bing for the image
                        List<BingSearchResult> newImageParts = await bingImageSearchAPI.PerformBingImageSearch(cognitiveServicesSubscriptionKey,
                                                                        cognitiveServicesBingSearchUriBase, cognitiveServicesImageAnalysisUriBase,
                                                                        searchTerm, 1, 10, tagFilter);
                        //Process the image parts
                        if (newImageParts.Any())
                        {
                            //Save the new part into the custom part images database table
                            PartImages newPartImages = new PartImages
                            {
                                PartNum = item.PartNum,
                                SourceImageUrl = newImageParts[0].ImageUrl,
                                ColorId = item.ColorId
                            };
                            await _repoPartImages.SavePartImage(newPartImages);

                            //Download the image into storage
                            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(storageContainerPartImagesName);
                            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(newImageName);
                            HttpWebRequest request = HttpWebRequest.CreateHttp(newImageParts[0].ImageUrl);
                            using (WebResponse response = await request.GetResponseAsync())
                            using (Stream dataStream = response.GetResponseStream())
                            {
                                await blob.UploadFromStreamAsync(dataStream);
                            }


                        }
                    }
                }
            }

            //TODO:
            //1.1.1. Farm out all parts in a set to functions to process quickly?
            //4. Log this all somehow? Perhaps to a page where we can see all of the sets, pieces, method uses, and the resultant image?
            //4.1. Need to have an override if the wrong part is found - probably from the log page

            return true;
        }

    }
}