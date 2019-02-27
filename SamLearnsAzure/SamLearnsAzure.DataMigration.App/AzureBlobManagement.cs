using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SamLearnsAzure.DataMigration.App
{
    public static class AzureBlobManagement
    {
        //Load up to the storage account, adapted from the Azure quick start for blob storage: 
        //https://github.com/Azure-Samples/storage-blobs-dotnet-quickstart/blob/master/storage-blobs-dotnet-quickstart/Program.cs
        public static async Task UploadFilesToStorageAccountBlobs(string storageConnectionString, string sourceContainerName, string tempFolderLocation, List<string> files, bool filesHaveFullPath, string partsContainerName)
        {
            CloudBlobContainer cloudBlobContainer = null;
            //string sourceFile = null;
            //string destinationFile = null;

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a new container  
                    cloudBlobContainer = cloudBlobClient.GetContainerReference(sourceContainerName);
                    bool containerExists = cloudBlobContainer == null || await cloudBlobContainer.ExistsAsync();
                    if (containerExists == false)
                    {
                        await cloudBlobContainer.CreateAsync();
                        Console.WriteLine("Created container '{0}'", cloudBlobContainer.Name);
                    }
                   // Set the permissions so the blobs are offs. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Off
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    //Create the parts location, if it doesn't already exist
                    CloudBlobContainer partsContainer = cloudBlobClient.GetContainerReference(partsContainerName);
                    bool containerExists2 = partsContainer == null || await partsContainer.ExistsAsync();
                    if (containerExists2 == false)
                    {
                        await partsContainer.CreateAsync();
                        Console.WriteLine("Created container '{0}'", partsContainer.Name);
                    }
                    // Set the permissions so the blobs are offs. 
                    BlobContainerPermissions permissions2 = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Off
                    };
                    await partsContainer.SetPermissionsAsync(permissions2);
                    

                    foreach (string file in files)
                    {
                        string fileToUpload = file;
                        if (filesHaveFullPath == false)
                        {
                            fileToUpload = tempFolderLocation + @"\" + file;
                        }
                        Console.WriteLine("Uploading to Blob storage as blob '{0}'", file);

                        // Get a reference to the blob address, then upload the file to the blob.
                        // Use the value of localFileName for the blob name.
                        if (File.Exists(fileToUpload) == true)
                        {
                            //Now strip the full path off so we can store any folders with the files we extracted
                            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileToUpload.Replace(tempFolderLocation + @"\", ""));
                            await cloudBlockBlob.UploadFromFileAsync(fileToUpload);
                        }
                        else
                        {
                            Console.WriteLine("File '" + fileToUpload + "' not found...");
                        }
                    }

                    // List the blobs in the container.
                    Console.WriteLine("Listing blobs in container.");
                    List<string> filesInBlob = await AzureBlobManagement.ListBlobs(storageConnectionString, sourceContainerName);
                    foreach (string item in filesInBlob)
                    {
                        Console.WriteLine(item);
                    }

                    //BlobContinuationToken blobContinuationToken = null;
                    //do
                    //{
                    //    var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    //    // Get the value of the continuation token returned by the listing call.
                    //    blobContinuationToken = results.ContinuationToken;
                    //    foreach (IListBlobItem item in results.Results)
                    //    {
                    //        Console.WriteLine(item.Uri);
                    //    }
                    //} while (blobContinuationToken != null); // Loop while the continuation token is not null.
                    ////Console.WriteLine();
                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }
            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }
        }


        private static async Task<List<string>> ListBlobs(string storageConnectionString, string containerName)
        {
            CloudBlobContainer cloudBlobContainer = null;
            List<string> files = new List<string>();

            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

                // List the blobs in the container.
                BlobContinuationToken continuationToken = null;
                var blobResultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync(continuationToken);
                files = blobResultSegment.Results.Select(i => i.Uri.Segments.Last()).ToList();

            }

            return files;
        }

        public static async Task UnzipBlob(string storageConnectionString, string sourceContainerName, string destinationContainerName, string fileName)
        {
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

        }

        public static async Task UnZipFilesInStorageBlobs(string storageConnectionString, string sourceContainerName, string destinationContainerName)
        {
            //Get a list of zip files in the blob
            List<string> files = await AzureBlobManagement.ListBlobs(storageConnectionString, sourceContainerName);

            //unzip each file
            foreach (string zipFile in files)
            {
                await AzureBlobManagement.UnzipBlob(storageConnectionString, sourceContainerName, destinationContainerName, zipFile);
            }
        }

        public static async Task UnZipFilesWithFunction(string storageConnectionString, string sourceContainerName, string destinationContainerName, string functionURL)
        {
            //Get a list of zip files in the blob
            List<string> files = await AzureBlobManagement.ListBlobs(storageConnectionString, sourceContainerName);

            //Call the function
            foreach (string file in files)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(functionURL);
                //client.SendAsync(file, sourceContainerName)
                HttpResponseMessage response = await client.GetAsync("/api/UnzipFileInBlob?code=pXUChJoDKFGZ9esg8RFMapWp/9YB1Fq4MTCMcsdfBt7n6QNmIoaDfw==&source=" + sourceContainerName + "&destination=" + destinationContainerName + "&file=" + file);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(file + " processed with code: " + response.StatusCode);
            }

        }
    }
}
