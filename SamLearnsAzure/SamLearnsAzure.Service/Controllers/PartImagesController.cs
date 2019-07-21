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
using static SamLearnsAzure.Service.AI.BingImageSearch;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartImagesController : ControllerBase
    {
        private readonly IPartImagesRepository _repo;
        private readonly IRedisService _redisService;
        private readonly IConfiguration _configuration;

        public PartImagesController(IPartImagesRepository repo, IRedisService redisService, IConfiguration configuration)
        {
            _repo = repo;
            _redisService = redisService;
            _configuration = configuration;
        }

        [HttpGet("GetPartImages")]
        public async Task<List<PartImages>> GetPartImages(bool useCache = true)
        {
            List<PartImages> results = await _repo.GetPartImages(_redisService, useCache);

            return results;
        }

        [HttpGet("GetPartImage")]
        public async Task<PartImages> GetPartImage(string partNum, bool useCache = true)
        {
            PartImages result = await _repo.GetPartImage(_redisService, useCache, partNum);

            return result;
        }

        [HttpGet("SavePartImage")]
        public async Task<PartImages> SavePartImage(string partNum, string sourceImage, int colorId)
        {
            PartImages newPartImage = new PartImages
            {
                PartNum = partNum,
                SourceImage = sourceImage,
                ColorId = colorId
            };
            PartImages result = await _repo.SavePartImage(newPartImage);

            return result;
        }

        [HttpGet("SearchForPotentialPartImages")]
        public async Task<List<PartImages>> SearchForPotentialPartImages(string partNum, int colorId, string colorName, int resultsToReturn = 1, int resultsToSearch = 1)
        {
            string cognitiveServicesSubscriptionKey = _configuration["CognitiveServicesSubscriptionKey"]; // The subscription key is coming from key vault
            string cognitiveServicesBingSearchUriBase = _configuration["AppSettings:CognitiveServicesBingSearchUriBase"];
            string cognitiveServicesImageAnalysisUriBase = _configuration["AppSettings:CognitiveServicesImageAnalysisUriBase"];
            string tagFilter = "lego";
            string searchTerm = partNum + " lego " + colorName;

            //1. Get image from Bing Image Search API
            BingImageSearch bingImageSearchAI = new BingImageSearch();
            List<BingSearchResult> images = await bingImageSearchAI.PerformBingImageSearch(cognitiveServicesSubscriptionKey,
                cognitiveServicesBingSearchUriBase, cognitiveServicesImageAnalysisUriBase,
                searchTerm, resultsToReturn, resultsToSearch, tagFilter);

            //2. Process the results
            List<PartImages> results = new List<PartImages>();
            foreach (BingSearchResult item in images)
            {
                PartImages newImage = new PartImages
                {
                    PartNum = partNum,
                    ColorId = colorId,
                    SourceImage = item.ImageUrl
                };
                results.Add(newImage);
            }

            return results;
        }


    }
}