using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using SamLearnsAzure.Web.Models;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<IActionResult> Set(string setnum)
        {
            Sets set = await _ServiceApiClient.GetSet(setnum);
            List<SetParts> setParts = await _ServiceApiClient.GetSetParts(setnum);

            SetViewModel setViewModel = new SetViewModel
            {
                Set = set,
                SetParts = setParts,
                BaseSetPartsImagesStorageURL = _configuration["AppSettings:PartImagesStorageURL"]
            };

            return View(setViewModel);
        }

        public IActionResult CDNTest()
        {
            CdnTestViewModel cdnTestViewModel = new CdnTestViewModel
            {
                BaseSetPartsImagesStorageURL = _configuration["AppSettings:PartImagesStorageURL"],
                BaseSetPartsImagesCDNURL = _configuration["AppSettings:PartImagesStorageCDNURL"]
            };

            return View(cdnTestViewModel);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

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
