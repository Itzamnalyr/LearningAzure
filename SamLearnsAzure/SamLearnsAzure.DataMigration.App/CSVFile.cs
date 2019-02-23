using System;
using System.Collections.Generic;
using System.Text;

namespace SamLearnsAzure.DataMigration.App
{
    public class CSVFile
    {
        public int NumberOfColumns;
        public List<int> StringColumns;
        public int ColumnToSquash;
        public List<int> BooleanColumns;
        public string FileName;

        public CSVFile()
        {
            StringColumns = new List<int>();
            BooleanColumns = new List<int>();
        }
    }
}
