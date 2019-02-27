using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace SamLearnsAzure.DataMigration.Function
{
    public static class AzureBlobManagement
    {      

        public static async Task<int> UnzipBlob(string storageConnectionString, string sourceContainerName, string destinationContainerName, string fileName)
        {
            int totalZips = 0;
            CloudBlobContainer cloudBlobSourceContainer = null;
            CloudBlobContainer cloudBlobDestinationContainer = null;

            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                cloudBlobSourceContainer = cloudBlobClient.GetContainerReference(sourceContainerName);

                // Retrieve reference to the blob - zip file which we wanted to extract 
                CloudBlockBlob blockBlob = cloudBlobSourceContainer.GetBlockBlobReference(fileName);

                // Create a new container  
                cloudBlobDestinationContainer = cloudBlobClient.GetContainerReference(destinationContainerName);
                bool containerExists = cloudBlobDestinationContainer == null || await cloudBlobDestinationContainer.ExistsAsync();
                if (containerExists == false)
                {
                    await cloudBlobDestinationContainer.CreateAsync();
                    Console.WriteLine("Created container '{0}'", cloudBlobDestinationContainer.Name);
                }

                // Save blob(zip file) contents to a Memory Stream.
                using (MemoryStream zipBlobFileStream = new MemoryStream())
                {
                    await blockBlob.DownloadToStreamAsync(zipBlobFileStream);
                    await zipBlobFileStream.FlushAsync();
                    zipBlobFileStream.Position = 0;

                    //use ZipArchive from System.IO.Compression to extract all the files from zip file
                    using (ZipArchive zip = new ZipArchive(zipBlobFileStream))
                    {
                        Console.WriteLine("Extracting {0} files from zip '{1}', at {2}", zip.Entries.Count, fileName, DateTime.Now.ToString());
                        //Each entry here represents an individual file or a folder
                        int i = 0;
                        totalZips = zip.Entries.Count;
                        foreach (ZipArchiveEntry entry in zip.Entries)
                        {
                            i++;
                            Console.Write("Unzipping image {0}/{1}", i, zip.Entries.Count);
                            Console.Write("\r");

                            //creating an empty file (blockBlob) for the actual file with the folder name (from the zip) and file name
                            string destinationFile = fileName.Replace(".zip", "").Replace("parts_", "") + "/" + entry.FullName;
                            CloudBlockBlob blob = cloudBlobDestinationContainer.GetBlockBlobReference(destinationFile);
                            using (Stream stream = entry.Open())
                            {
                                //check for file or folder and update the above blob reference with actual content from stream
                                if (entry.Length > 0)
                                {
                                    await blob.UploadFromStreamAsync(stream);
                                }
                            }
                        }
                    }
                }
            }

            return totalZips;

        }

    }
}
