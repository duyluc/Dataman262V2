using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
using CsvHelper;

namespace DATAMAN262.Helper
{
    public class LogCSV
    {
        public List<string> a = new List<string>();
        static public string SaveFolderPath = ".\\LogCSV";
        /// <summary>
        /// Contruction
        /// </summary>
        public LogCSV()
        {

        }

        static public bool CheckFileExit(string filename)
        {
            string path = Path.Combine(SaveFolderPath, filename);
            if (File.Exists(path)) return true;
            else return false;
        }
    }
}
