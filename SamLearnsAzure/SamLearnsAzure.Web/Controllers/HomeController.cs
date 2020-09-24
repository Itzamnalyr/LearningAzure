using System;
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
        private readonly IFeatureFlagsServiceApiClient _featureFlagsServiceApiClient;

        public HomeController(IServiceApiClient ServiceApiClient, IConfiguration configuration, IFeatureFlagsServiceApiClient featureFlagsServiceApiClient)
        {
            _ServiceApiClient = ServiceApiClient;
            _configuration = configuration;
            _featureFlagsServiceApiClient = featureFlagsServiceApiClient;
        }

        [HttpGet, HttpHead]
        public async Task<IActionResult> Index()
        {
            int ownerId = 1; //Sam
            List<OwnerSets> ownerSets = await _ServiceApiClient.GetOwnerSets(ownerId);

            IndexViewModel indexPageData = new IndexViewModel
            (
                environment: _configuration["AppSettings:Environment"],
                ownerSets: ownerSets
            );

            //Divide by zero feature flag
            bool featureFlagResult = false;
            if (_featureFlagsServiceApiClient != null)
            {
                featureFlagResult = await _featureFlagsServiceApiClient.CheckFeatureFlag("DivideByZero", _configuration["AppSettings:Environment"].ToString());
            }

            if (featureFlagResult == true)
            {
                int i = 1;
                int j = 0;
                Console.WriteLine(i / j);
            }

            return View(indexPageData);
        }

        [HttpGet]
        public async Task<IActionResult> Set(string setnum)
        {
            Sets set = await _ServiceApiClient.GetSet(setnum);
            SetImages setImage = await _ServiceApiClient.GetSetImage(setnum);
            List<SetParts> setParts = await _ServiceApiClient.GetSetParts(setnum);

            SetViewModel setViewModel = new SetViewModel
            (
                set: set,
                setImage: setImage,
                setParts: setParts,
                baseSetPartsImagesStorageURL: _configuration["AppSettings:ImagesStorageCDNURL"] + _configuration["AppSettings:PartImagesContainerName"],
                baseSetImagesStorageURL: _configuration["AppSettings:ImagesStorageURL"] + _configuration["AppSettings:SetImagesContainerName"]
            );

            return View(setViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateSetImage(string setnum, int resultsToReturn, int resultsToSearch)
        {
            Sets set = await _ServiceApiClient.GetSet(setnum);
            List<SetImages> setImages = await _ServiceApiClient.GetSetImages(setnum, resultsToReturn, resultsToSearch);

            UpdateSetImageViewModel updateSetImageModel = new UpdateSetImageViewModel
            (
                set: set,
                potentialSetImages: setImages,
                baseSetImagesStorageURL: ""
            );

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

            PartImagesViewModel partImagesViewModel = new PartImagesViewModel(
                partImages,
                _configuration["AppSettings:ImagesStorageURL"] + _configuration["AppSettings:PartImagesContainerName"]);

            return View(partImagesViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdatePartImage(string setNum, string partNum, int colorId, string colorName)
        {
            int resultsToReturn = 10;
            int resultsToSearch = 20;

            Sets set = await _ServiceApiClient.GetSet(setNum);
            List<SetParts> setParts = await _ServiceApiClient.GetSetParts(setNum);
            List<PartImages> partImages = await _ServiceApiClient.SearchForPotentialPartImages(partNum, colorId, colorName, resultsToReturn, resultsToSearch);

            UpdatePartImageViewModel updatePartImageModel = new UpdatePartImageViewModel(
                set: set,
                currentSetPart: setParts.SingleOrDefault(t => t.PartNum == partNum),
                potentialSetParts: partImages,
                basePartsImagesStorageURL: _configuration["AppSettings:ImagesStorageURL"] + _configuration["AppSettings:PartImagesContainerName"]
            );

            return View(updatePartImageModel);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public IActionResult CDNTest()
        {
            CdnTestViewModel cdnTestViewModel = new CdnTestViewModel
            (
                baseSetPartsImagesStorageURL: _configuration["AppSettings:ImagesStorageURL"] + _configuration["AppSettings:PartImagesContainerName"],
                baseSetPartsImagesCDNURL: _configuration["AppSettings:ImagesStorageCDNURL"] + _configuration["AppSettings:PartImagesContainerName"]
            );

            return View(cdnTestViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Sam Learns Azure.";

            //About upgrade feature flag
            bool featureFlagResult = false;
            if (_featureFlagsServiceApiClient != null)
            {
                featureFlagResult = await _featureFlagsServiceApiClient.CheckFeatureFlag("SiteAboutPageUpgrade", _configuration["AppSettings:Environment"].ToString());
            }

            return View(featureFlagResult);
        }


        [HttpHead]
        public IActionResult HealthProbe()
        {
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
            return View(new ErrorViewModel(Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }
    }
}
