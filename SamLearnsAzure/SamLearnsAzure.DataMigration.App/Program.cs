using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SamLearnsAzure.DataMigration.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                //Start the timer
                System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

                //Get configuration values from the appsettings.json file
                IConfiguration config = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json", true, true)
                      .Build();
                string partFilesToDownloadURL = config["partFilesToDownloadURL"];
                string csvFilesToDownloadURL = config["csvFilesToDownloadURL"];
                string storageConnectionString = config["storageConnectionString"];
                string tempFolderLocationCSVFiles = Path.GetTempPath() + config["tempFolderLocationCSVFiles"];
                string tempFolderLocationParts = Path.GetTempPath() + config["tempFolderLocationParts"];
                string tempFolderLocationPartsUnZipped = Path.GetTempPath() + config["tempFolderLocationPartsUnZipped"];
                string functionURL = config["functionURL"];
                string zippedPartsContainerName = "zippedparts";
                string partsContainerName = "partimages";

                bool uploadCSVFiles = true; //This should normally be on
                bool uploadNewPartZips = true; //This should normally be on
                bool unzipPartsWithHTTPFunction = false; //This should normally be off, it's for debugging


                //Confirm that the temp folder can be accessed and is clear
                if ((uploadCSVFiles == true && LocalFileManagement.DeleteAllSubFoldersInTargetFolder(tempFolderLocationCSVFiles) == false) ||
                    (uploadNewPartZips == true && LocalFileManagement.DeleteAllSubFoldersInTargetFolder(tempFolderLocationParts) == false) ||
                    (uploadNewPartZips == true && LocalFileManagement.DeleteAllSubFoldersInTargetFolder(tempFolderLocationPartsUnZipped) == false))
                {
                    PressAKeyToExit("Temp folder preparation failed - process aborted");
                }
                else
                {
                    if (uploadCSVFiles == true)
                    {
                        //Get csv files to process
                        List<CSVFile> csvFiles = GetCSVFiles();
                        List<string> csvFileNames = GetCSVFileNames(csvFiles);
                        //Download all files from URL to temp foldter
                        await LocalFileManagement.DownloadFilesToTempFolder(csvFilesToDownloadURL, tempFolderLocationCSVFiles, csvFileNames);
                        //Process the CSV files to clean them up
                        foreach (CSVFile item in csvFiles)
                        {
                            CSVFileProcessor fp = new CSVFileProcessor();
                            await fp.ProcessCSVFile(item.NumberOfColumns, item.StringColumns, item.ColumnToSquash, item.BooleanColumns, item.FileName, tempFolderLocationCSVFiles);
                        }
                        //Upload files to Azure Blob
                        await AzureBlobManagement.UploadFilesToStorageAccountBlobs(storageConnectionString, "csvfiles", tempFolderLocationCSVFiles, csvFileNames, false, partsContainerName);
                    }

                    if (uploadNewPartZips == true)
                    {
                        //Get list of colors from database
                        List<string> colorFiles = GetColorFiles();

                        //Download all files from URL to temp folder
                        await LocalFileManagement.DownloadFilesToTempFolder(partFilesToDownloadURL, tempFolderLocationParts, colorFiles);

                        //Upload zipped files to Azure Blob
                        await AzureBlobManagement.UploadFilesToStorageAccountBlobs(storageConnectionString, zippedPartsContainerName, tempFolderLocationParts, colorFiles, false, partsContainerName);

                    }

                    if (unzipPartsWithHTTPFunction == true)
                    {
                        //Send Http request to function to process zip file
                        await AzureBlobManagement.UnZipFilesWithFunction(storageConnectionString, zippedPartsContainerName, partsContainerName, functionURL);
                    }

                    //Bonus: Trigger azure function to start data factory import
                    //Bonus: If new color detected, run this process again

                    // stop the timer
                    watch.Stop();
                    double elapsedSeconds = watch.Elapsed.TotalSeconds;

                    PressAKeyToExit("Successfully uploaded in " + elapsedSeconds.ToString() + "s!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                PressAKeyToExit(ex.ToString());
            }
        }

        private static List<string> GetCSVFileNames(List<CSVFile> csvFiles)
        {
            List<string> csvFileNames = new List<string>();
            foreach (CSVFile item in csvFiles)
            {
                csvFileNames.Add(item.FileName);
            }

            return csvFileNames;
        }

        private static List<CSVFile> GetCSVFiles()
        {
            List<CSVFile> csvFiles = new List<CSVFile>();

            //colors.csv
            CSVFile csvColorsFile = new CSVFile
            {
                NumberOfColumns = 4,
                FileName = "colors.csv",
            };
            csvColorsFile.StringColumns.Add(2);
            csvColorsFile.StringColumns.Add(3);
            csvColorsFile.BooleanColumns.Add(4);
            csvFiles.Add(csvColorsFile);

            //inventories.csv
            CSVFile csvInventoriesFile = new CSVFile
            {
                NumberOfColumns = 3,
                FileName = "inventories.csv",
            };
            csvInventoriesFile.StringColumns.Add(3);
            csvFiles.Add(csvInventoriesFile);

            //inventory_parts.csv
            CSVFile csvInventoryPartsFile = new CSVFile
            {
                NumberOfColumns = 5,
                FileName = "inventory_parts.csv",
            };
            csvInventoryPartsFile.StringColumns.Add(2);
            csvInventoryPartsFile.BooleanColumns.Add(5);
            csvFiles.Add(csvInventoryPartsFile);

            //inventory_sets.csv
            CSVFile csvInventorySetsFile = new CSVFile
            {
                NumberOfColumns = 3,
                FileName = "inventory_sets.csv",
            };
            csvInventorySetsFile.StringColumns.Add(2);
            csvFiles.Add(csvInventorySetsFile);

            //part_categories.csv
            CSVFile csvPartCategoriesFile = new CSVFile
            {
                NumberOfColumns = 2,
                FileName = "part_categories.csv",
            };
            csvPartCategoriesFile.StringColumns.Add(2);
            csvPartCategoriesFile.ColumnToSquash = 2;
            csvFiles.Add(csvPartCategoriesFile);

            //part_relationships.csv
            CSVFile csvPartRelationshipsFile = new CSVFile
            {
                NumberOfColumns = 3,
                FileName = "part_relationships.csv",
            };
            csvPartRelationshipsFile.StringColumns.Add(1);
            csvPartRelationshipsFile.StringColumns.Add(2);
            csvPartRelationshipsFile.StringColumns.Add(3);
            csvFiles.Add(csvPartRelationshipsFile);

            //parts.csv
            CSVFile csvPartsFile = new CSVFile
            {
                NumberOfColumns = 4,
                FileName = "parts.csv",
            };
            csvPartsFile.StringColumns.Add(1);
            csvPartsFile.StringColumns.Add(2);
            csvPartsFile.ColumnToSquash = 2;
            csvFiles.Add(csvPartsFile);

            //sets.csv
            CSVFile csvSetsFile = new CSVFile
            {
                NumberOfColumns = 5,
                FileName = "sets.csv",
            };
            csvSetsFile.StringColumns.Add(1);
            csvSetsFile.StringColumns.Add(2);
            csvSetsFile.ColumnToSquash = 2;
            csvFiles.Add(csvSetsFile);

            //themes.csv
            CSVFile csvThemesFile = new CSVFile
            {
                NumberOfColumns = 3,
                FileName = "themes.csv",
            };
            csvThemesFile.StringColumns.Add(2);
            csvFiles.Add(csvThemesFile);

            return csvFiles;
        }

        //TODO: Use a database connection string instead of a static list
        private static List<string> GetColorFiles()
        {
            List<string> colorFiles = new List<string>
            {
                "parts_-1.zip",
                "parts_0.zip",
                "parts_1.zip",
                "parts_2.zip",
                "parts_3.zip",
                "parts_4.zip",
                "parts_5.zip",
                "parts_6.zip",
                "parts_7.zip",
                "parts_8.zip",
                "parts_9.zip",
                "parts_10.zip",
                "parts_11.zip",
                "parts_12.zip",
                "parts_13.zip",
                "parts_14.zip",
                "parts_15.zip",
                "parts_17.zip",
                "parts_18.zip",
                "parts_19.zip",
                "parts_20.zip",
                "parts_21.zip",
                "parts_22.zip",
                "parts_23.zip",
                "parts_25.zip",
                "parts_26.zip",
                "parts_27.zip",
                "parts_28.zip",
                "parts_29.zip",
                "parts_30.zip",
                "parts_31.zip",
                "parts_32.zip",
                "parts_33.zip",
                "parts_34.zip",
                "parts_35.zip",
                "parts_36.zip",
                "parts_40.zip",
                "parts_41.zip",
                "parts_42.zip",
                "parts_43.zip",
                "parts_45.zip",
                "parts_46.zip",
                "parts_47.zip",
                "parts_52.zip",
                "parts_54.zip",
                "parts_57.zip",
                "parts_60.zip",
                "parts_61.zip",
                "parts_62.zip",
                "parts_63.zip",
                "parts_64.zip",
                "parts_68.zip",
                "parts_69.zip",
                "parts_70.zip",
                "parts_71.zip",
                "parts_72.zip",
                "parts_73.zip",
                "parts_74.zip",
                "parts_75.zip",
                "parts_76.zip",
                "parts_77.zip",
                "parts_78.zip",
                "parts_79.zip",
                "parts_80.zip",
                "parts_81.zip",
                "parts_82.zip",
                "parts_84.zip",
                "parts_85.zip",
                "parts_86.zip",
                "parts_89.zip",
                "parts_92.zip",
                "parts_100.zip",
                "parts_110.zip",
                "parts_112.zip",
                "parts_114.zip",
                "parts_115.zip",
                "parts_117.zip",
                "parts_118.zip",
                "parts_120.zip",
                "parts_125.zip",
                "parts_129.zip",
                "parts_132.zip",
                "parts_133.zip",
                "parts_134.zip",
                "parts_135.zip",
                "parts_137.zip",
                "parts_142.zip",
                "parts_143.zip",
                "parts_148.zip",
                "parts_150.zip",
                "parts_151.zip",
                "parts_158.zip",
                "parts_178.zip",
                "parts_179.zip",
                "parts_182.zip",
                "parts_183.zip",
                "parts_191.zip",
                "parts_212.zip",
                "parts_216.zip",
                "parts_226.zip",
                "parts_230.zip",
                "parts_232.zip",
                "parts_236.zip",
                "parts_272.zip",
                "parts_288.zip",
                "parts_294.zip",
                "parts_297.zip",
                "parts_308.zip",
                "parts_313.zip",
                "parts_320.zip",
                "parts_321.zip",
                "parts_322.zip",
                "parts_323.zip",
                "parts_326.zip",
                "parts_334.zip",
                "parts_335.zip",
                "parts_351.zip",
                "parts_366.zip",
                "parts_373.zip",
                "parts_378.zip",
                "parts_379.zip",
                "parts_383.zip",
                "parts_450.zip",
                "parts_462.zip",
                "parts_484.zip",
                "parts_503.zip",
                "parts_1000.zip",
                "parts_1001.zip",
                "parts_1002.zip",
                "parts_1003.zip",
                "parts_1004.zip",
                "parts_1005.zip",
                "parts_1006.zip",
                "parts_1007.zip",
                "parts_9999.zip"
            };

            return colorFiles;
        }

        private static void PressAKeyToExit(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
