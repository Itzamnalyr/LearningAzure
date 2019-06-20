using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using SamLearnsAzure.Web.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

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
                BaseSetImagesStorageURL = _configuration["AppSettings:ImagesStorageCDNURL"] + _configuration["AppSettings:SetImagesContainerName"]
            };

            return View(setViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateImage(string setnum)
        {
            Sets set = await _ServiceApiClient.GetSet(setnum);
            List<SetImages> setImages = await _ServiceApiClient.GetSetImages(setnum);

            UpdateImageViewModel updateImageModel = new UpdateImageViewModel
            {
                Set = set,
                SetImages = setImages
            };

            return View(updateImageModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateImageSave(string setNum, string imageUrl)
        {
            await _ServiceApiClient.SaveSetImage(setNum, imageUrl);

            return RedirectToAction("Set", new { setNum = setNum });
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
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            ViewData["Message"] = "Your privacy page.";
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
