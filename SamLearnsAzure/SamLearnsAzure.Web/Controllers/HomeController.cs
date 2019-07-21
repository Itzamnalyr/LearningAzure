﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using SamLearnsAzure.Web.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Storage;

namespace SamLearnsAzure.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServiceApiClient _ServiceApiClient;
        private readonly IConfiguration _configuration;

        public HomeController(IServiceApiClient ServiceApiClient, IConfiguration configuration)
        {
            _ServiceApiClient = ServiceApiClient;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int ownerId = 1; //Sam
            List<OwnerSets> ownerSets = await _ServiceApiClient.GetOwnerSets(ownerId);

            IndexViewModel indexPageData = new IndexViewModel
            {
                Environment = _configuration["AppSettings:Environment"],
                OwnerSets = ownerSets
            };

            return View(indexPageData);
        }

        [HttpGet]
        public async Task<IActionResult> Set(string setnum)
        {
            Sets set = await _ServiceApiClient.GetSet(setnum);
            SetImages setImage = await _ServiceApiClient.GetSetImage(setnum);
            List<SetParts> setParts = await _ServiceApiClient.GetSetParts(setnum);

            SetViewModel setViewModel = new SetViewModel
            {
                Set = set,
                SetImage = setImage,
                SetParts = setParts,
                BaseSetPartsImagesStorageURL = _configuration["AppSettings:ImagesStorageCDNURL"] + _configuration["AppSettings:PartImagesContainerName"],
                BaseSetImagesStorageURL = _configuration["AppSettings:ImagesStorageURL"] + _configuration["AppSettings:SetImagesContainerName"]
            };

            return View(setViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateSetImage(string setnum, int resultsToReturn, int resultsToSearch)
        {
            Sets set = await _ServiceApiClient.GetSet(setnum);
            List<SetImages> setImages = await _ServiceApiClient.GetSetImages(setnum, resultsToReturn, resultsToSearch);

            UpdateSetImageViewModel updateSetImageModel = new UpdateSetImageViewModel
            {
                Set = set,
                PotentialSetImages = setImages
            };

            return View(updateSetImageModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateSetImageSave(string setNum, string imageUrl)
        {
            await _ServiceApiClient.SaveSetImage(setNum, imageUrl);

            return RedirectToAction("Set", new { setNum });
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SearchForMissingParts(string setNum)
        {
            await _ServiceApiClient.SearchForMissingParts(setNum);

            return RedirectToAction("Set", new { setNum });
        }

        //[Authorize]
        [HttpGet]
        public IActionResult Admin()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PartImages()
        {

            //Get all custom, downloaded parts
            List<PartImages> partImages = await _ServiceApiClient.GetPartImages();

            PartImagesViewModel partImagesViewModel = new PartImagesViewModel
            {
                PartImages = partImages,
                BasePartsImagesStorageURL = _configuration["AppSettings:ImagesStorageURL"] + _configuration["AppSettings:PartImagesContainerName"]
            };

            return View(partImagesViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdatePartImage(string partNum, int colorId, string colorName)
        {
            int resultsToReturn = 10;
            int resultsToSearch = 20;

            List<SetParts> setParts = await _ServiceApiClient.GetSetParts(partNum);
            List<PartImages> partImages = await _ServiceApiClient.SearchForPotentialPartImages(partNum, colorId, colorName, resultsToReturn, resultsToSearch); 

            UpdatePartImageViewModel updatePartImageModel = new UpdatePartImageViewModel
            {
                CurrentSetPart = setParts.SingleOrDefault(t => t.PartNum == partNum),
                PotentialSetParts = partImages
            };

            return View(updatePartImageModel);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public IActionResult CDNTest()
        {
            CdnTestViewModel cdnTestViewModel = new CdnTestViewModel
            {
                BaseSetPartsImagesStorageURL = _configuration["AppSettings:ImagesStorageURL"] + _configuration["AppSettings:PartImagesContainerName"],
                BaseSetPartsImagesCDNURL = _configuration["AppSettings:ImagesStorageCDNURL"] + _configuration["AppSettings:PartImagesContainerName"]
            };

            return View(cdnTestViewModel);
        }

        [HttpGet]
        public IActionResult About()
        {
            ViewData["Message"] = "Sam Learns Azure.";
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            ViewData["Message"] = "Sam Learns Azure privacy page.";
            return View();
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
