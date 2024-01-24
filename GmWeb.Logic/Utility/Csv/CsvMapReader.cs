using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Csv
{
    public class CsvMapReader<TRecord, TRecordMap> : CsvReader<TRecord>
        where TRecord : CsvRecord
        where TRecordMap : CsvRecordMap<TRecord, TRecordMap>
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected TRecordMap RecordMap { get; private set; }

        public CsvMapReader(string FilePath, string Delimiter = ",", int HeaderCount = 1)
            : this(new FileInfo(FilePath), Delimiter, HeaderCount)
        { }
        public CsvMapReader(FileInfo File, string Delimiter = ",", int HeaderCount = 1)
            : base(File, Delimiter, HeaderCount)
        {
            this.RecordMap = this.WrappedCsvReader.Configuration.RegisterClassMap<TRecordMap>().Init();
        }
    }
}
