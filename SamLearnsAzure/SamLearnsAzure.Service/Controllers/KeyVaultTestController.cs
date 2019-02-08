using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyVaultTestController : ControllerBase
    {
        private IConfiguration _configuration;

        public KeyVaultTestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GetApplicationInsightsInstrumentationKey")]
        public string GetApplicationInsightsInstrumentationKey()
        {
            return _configuration["ApplicationInsights:InstrumentationKey"];
        }

        [HttpGet("GetStorageKey")]
        public string GetStorageKey()
        {
            return _configuration["StorageAccountKey"];
        }
    }
}
