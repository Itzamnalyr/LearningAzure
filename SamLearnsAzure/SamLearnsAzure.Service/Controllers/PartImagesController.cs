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
        public async Task<List<PartImages>> GetPartImages()
        {
            List<PartImages> results = await _repo.GetPartImages(_redisService, false);

            return results;
        }

        [HttpGet("GetPartImage")]
        public async Task<PartImages> GetPartImage(string partNum)
        {
            PartImages result = await _repo.GetPartImage(_redisService, false, partNum);

            return result;
        }

        [HttpGet("SavePartImage")]
        public async Task<PartImages> SavePartImage(string partNum, string sourceImage, int colorId)
        {
            PartImages newPartImage = new PartImages
            {
                PartNum = partNum,
                SourceImageUrl = sourceImage,
                ColorId = colorId
            };
            PartImages result = await _repo.SavePartImage(newPartImage);

            return result;
        }


    }
}