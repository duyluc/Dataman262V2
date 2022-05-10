using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using System.IO;

namespace DATAMAN262.Helper
{
    public class DatabaseProvider
    {
        static public readonly string _databaseFolderPath = ".\\Database";
        static public string DatabaseFilePath = Path.Combine(_databaseFolderPath, $"Database.db");
        public class DatamanParam
        {
            public int Id { get; set; }
            public string DatamanId { get; set; }
            public string Ip { get; set; }
            public string LastTriggerId { get; set; }
            public Int32 TriggerCount { get; set; }
            public string LastResultId { get; set; }
            public Int32 ResultCount { get; set; }
            public string LastResult { get; set; }
        }

        public class Record
        {
            public int Id { get; set; }
            public string TimeLine { get; set; }
            public string DatamanId { get; set; }
            public string TriggerId { get; set; }
            public string ResultId { get; set; }
            public string Result { get; set; }
        }

        public class RecordView
        {
            public string TimeLine { get; set; }
            public string Result { get; set; }
        }

    }
}
