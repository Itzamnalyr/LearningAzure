using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SamLearnsAzure.DataMigration.App
{
    public class CSVFileProcessor
    {
        public async Task ProcessCSVFile(int numberOfColumns, List<int> stringColumns, int columnToSquash, List<int> booleanColumns, string fileName, string path)
        {
            //Create a new file we will write at the end
            StringBuilder newCSVFile = new StringBuilder();

            //Open the file.
            using (StreamReader reader = new StreamReader(path + @"\" + fileName))
            {
                Console.WriteLine("Reading clean CSV file '" + fileName + "'");
                // Loop through line by line splitting by comma
                int lineNumber = 0;
                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    string line = await reader.ReadLineAsync();
                    string[] values = SplitCSVStringWithQuotes(line);
                    if (values.Length > numberOfColumns)
                    {
                        //there are extra columns, we need to do special processing to squash the line back together again
                        //Debug.WriteLine(lineNumber.ToString() + ": Problem line: " + line);

                        //Take extra columns and squash them.
                        StringBuilder sbFix = new StringBuilder();
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (i >= columnToSquash && i <= values.Length - columnToSquash)
                            {
                                do
                                {
                                    sbFix.Append(values[i]);
                                    sbFix.Append(",");
                                    i++;
                                } while (i < values.Length);
                                //sbFix.Append(",");
                            }
                            else
                            {
                                sbFix.Append(values[i]);
                                sbFix.Append(",");
                            }
                        }
                        //Split the comma delimited CSV file into an array
                        values = SplitCSVStringWithQuotes(sbFix.ToString());
                        if (values.Length > numberOfColumns)
                        {
                            Debug.WriteLine(lineNumber.ToString() + ": Problem still needs to be squashed: " + line);
                        }
                        else
                        {
                            newCSVFile.Append(ProcessValue(lineNumber, values, stringColumns, booleanColumns));
                        }
                    }
                    else
                    {
                        newCSVFile.Append(ProcessValue(lineNumber, values, stringColumns, booleanColumns));
                    }
                }
                reader.Close();
            }

            if (newCSVFile.Length == 0)
            {
                throw new Exception("CSV file '" + fileName + "' was empty");
            }
            else
            {
                //Write the completed file back to the CSV file to overwrite the file we read earlier
                using (StreamWriter writer = new StreamWriter(path + @"\" + fileName))
                {
                    Console.WriteLine("Writing clean CSV file '" + fileName + "'");
                    writer.Write(newCSVFile);
                    writer.Close();
                }
            }
        }

        private string ProcessValue(int lineNumber, string[] values, List<int> stringColumns, List<int> booleanColumns)
        {
            StringBuilder sb = new StringBuilder();

            //It's a regularly formed column, push it back together again and save it
            for (int i = 0; i < values.Length; i++)
            {
                //Add string quote
                if (lineNumber > 1 && stringColumns.Contains(i + 1))
                {
                    if (values[i].StartsWith("\"") == false)
                    {
                        sb.Append("\"");
                    }
                }
                //Process Boolean values 
                if (lineNumber > 1 && booleanColumns.Contains(i + 1))
                {
                    if (values[i] == "t")
                    {
                        sb.Append("TRUE");
                    }
                    else
                    {
                        sb.Append("FALSE");
                    }
                }
                else //Load the value
                {
                    sb.Append(values[i]);
                }
                //Add string quote
                if (lineNumber > 1 && stringColumns.Contains(i + 1))
                {
                    if (values[i].StartsWith("\"") == false)
                    {
                        sb.Append("\"");
                    }
                }
                //Append the comma separated
                if (i < values.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append(Environment.NewLine);

            //Debug.WriteLine(lineNumber.ToString() + ": " + sb.ToString());
            return sb.ToString();
        }

        private string[] SplitCSVStringWithQuotes(string stringToSplit)
        {
            //https://stackoverflow.com/questions/6542996/how-to-split-csv-whose-columns-may-contain
            return Regex.Split(stringToSplit, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        }
    }
}
