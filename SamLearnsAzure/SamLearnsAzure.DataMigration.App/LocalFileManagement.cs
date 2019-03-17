using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamLearnsAzure.DataMigration.App
{
    public static class LocalFileManagement
    {
        public static bool DeleteAllSubFoldersInTargetFolder(string tempFolderLocation)
        {
            //Check that the folder is empty, deleting it if it already exists
            if (Directory.Exists(tempFolderLocation) == true)
            {
                Directory.Delete(tempFolderLocation, true);
                while (Directory.Exists(tempFolderLocation))
                {
                    Console.WriteLine("Deleting directory '" + tempFolderLocation + "'");
                    Thread.Sleep(1000);
                }
            }

            //Create the new folder
            Directory.CreateDirectory(tempFolderLocation);

            return true;
        }

        public async static Task<bool> DownloadFilesToTempFolder(string downloadURL, string tempFolderLocation, List<string> files)
        {
            foreach (string file in files)
            {

                string fileToDownload = downloadURL + file;
                Console.WriteLine("Downloading file '" + fileToDownload + "'");
                //Need to look and remove the items like a queue, but then skip to the next one and come back if there is a problem.
                //TODO: Fix this retry loop
                for (int retries = 0; retries < 5; retries++)
                {
                    try
                    {
                        byte[] fileBytes = await DownloadFile(fileToDownload);
                        string downloadedFile = tempFolderLocation + @"\" + file;
                        File.WriteAllBytes(downloadedFile, fileBytes);
                        FileInfo fileInfo = new FileInfo(downloadedFile);
                        break;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed downloading file - let's try again... (" + (retries + 1).ToString() + "/5");
                        if (retries == 4)
                        {
                            Console.WriteLine("Failed downloading file '" + fileToDownload + "!!'");
                        }
                    }
                }
            }

            return true;
        }

        public static bool UnZipFiles(string tempFolderLocation, string tempFolderUnZippedLocation, List<string> zipFiles)
        {
            foreach (string zipFile in zipFiles)
            {
                if (File.Exists(tempFolderLocation + @"\" + zipFile) == true)
                {
                    string colorId = zipFile.Replace("parts_", "").Replace(".zip", "");
                    string tempFolderUnZippedLocationColorId = tempFolderUnZippedLocation + @"\" + colorId;
                    Directory.CreateDirectory(tempFolderUnZippedLocationColorId);
                    //This line isn't required, but gives us more information for the console, and speed isn't a requirement here.
                    ZipArchive zip = ZipFile.OpenRead(tempFolderLocation + @"\" + zipFile);
                    Console.WriteLine("Unzipping " + zip.Entries.Count.ToString() + " files from zip '" + zipFile + "' to '" + tempFolderUnZippedLocationColorId + "'");
                    ZipFile.ExtractToDirectory(tempFolderLocation + @"\" + zipFile, tempFolderUnZippedLocationColorId, true);
                }
                else
                {
                    Console.WriteLine("File '" + tempFolderLocation + @"\" + zipFile + "' not found...");
                }
            }
            return true;
        }

        public static List<string> GetUnZippedFiles(string tempFolderUnZippedLocation)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(tempFolderUnZippedLocation, "*.png", SearchOption.AllDirectories));
            return files;
        }

        //Download the file
        private static async Task<byte[]> DownloadFile(string url)
        {
            //TODO: Fix this retry loop
            for (int retries = 0; retries < 5; retries++)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage result = await client.GetAsync(url))
                        {
                            if (result.IsSuccessStatusCode)
                            {
                                return await result.Content.ReadAsByteArrayAsync();
                            }
                        }
                    }
                }
                catch
                {
                    //do nothing, try again!
                    Console.WriteLine("Failed download - let's try again... (" + (retries + 1).ToString() + "/5");
                }
            }

            return null;
        }
    }
}
