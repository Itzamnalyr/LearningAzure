using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SamLearnsAzure.DataMigration.Function
{
    public static class UnzipFileHttpTrigger
    {
       
        [FunctionName("UnzipFileHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string storageConnectionString = Environment.GetEnvironmentVariable("storageConnectionString");
            string sourceContainerName = req.Query["source"];
            string destinationContainerName = req.Query["destination"];
            string file = req.Query["file"];

            //Start the timer
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            //Process the zip file
            int totalImages = await AzureBlobManagement.UnzipBlob(storageConnectionString, sourceContainerName, destinationContainerName, file);

            // stop the timer
            watch.Stop();
            double elapsedSeconds = watch.Elapsed.TotalSeconds;

            string returnString = $"Zip file '" + file + "' successfully processed " + totalImages + " files from " + sourceContainerName + " to " + destinationContainerName + " in " + elapsedSeconds.ToString() + " seconds.";

            return totalImages > 0
                ? (ActionResult)new OkObjectResult(returnString)
                : new BadRequestObjectResult("Zip file not processed");
        }
    }
    }
