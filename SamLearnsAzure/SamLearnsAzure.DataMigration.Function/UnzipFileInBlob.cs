using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace SamLearnsAzure.DataMigration.Function
{
    public static class UnzipFileInBlob
    {
        [FunctionName("UnzipFileInBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string storageConnectionString = Environment.GetEnvironmentVariable("storageConnectionString");
            //source=" + sourceContainerName + "&destination=" + destinationContainerName + "&file=" + file
            string sourceContainerName = req.Query["source"];
            string destinationContainerName = req.Query["destination"];
            string file = req.Query["file"];

            //Start the timer
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            int totalImages = await AzureBlobManagement.UnzipBlob(storageConnectionString, sourceContainerName, destinationContainerName, file);

            // stop the timer
            watch.Stop();
            double elapsedSeconds = watch.Elapsed.TotalSeconds;

            string returnString = $"Zip file '" + file + "' successfully processed " + totalImages + " zips from " + sourceContainerName + " to " + destinationContainerName + " in " + elapsedSeconds.ToString() + " seconds.";

            return totalImages > 0
                ? (ActionResult)new OkObjectResult(returnString)
                : new BadRequestObjectResult("Zip file not processed");
        }
    }
}