using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SamLearnsAzure.DataMigration.Function
{
    public static class UnzipFileBlobTrigger
    {
        [FunctionName("UnzipFileBlobTrigger")]
        public static async Task Run([BlobTrigger("zippedparts/{name}", 
            Connection = "storageConnectionString")]Stream myBlob, string name, ILogger log)
        {
            if (myBlob != null)
            {
                log.LogInformation($"C# Blob trigger function processing blob, Name:{name}, Size: {myBlob.Length} Bytes");
            }
            else
            {
                log.LogInformation($"C# Blob trigger function processing blob, Name:{name}, Size: 0 Bytes");
            }

            string storageConnectionString = Environment.GetEnvironmentVariable("storageConnectionString");
            string sourceContainerName = Environment.GetEnvironmentVariable("sourceContainer");
            string destinationContainerName = Environment.GetEnvironmentVariable("destinationContainer");

            //Start the timer
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            //Process the zip file
            int totalImages = await AzureBlobManagement.UnzipBlob(storageConnectionString, sourceContainerName, destinationContainerName, name);

            // stop the timer
            watch.Stop();
            double elapsedSeconds = watch.Elapsed.TotalSeconds;
            log.LogInformation($"Zip file '" + name + "' successfully processed " + totalImages + " files from " + sourceContainerName + " to " + destinationContainerName + " in " + elapsedSeconds.ToString() + " seconds.");
        }


    }
}
