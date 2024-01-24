using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Csv
{
    public class CsvReader<TRecord> : CsvReaderBase
        where TRecord : CsvRecord
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CsvReader(string FilePath, string Delimiter = ",", int HeaderCount = 1)
            : this(new FileInfo(FilePath), Delimiter, HeaderCount)
        { }
        public CsvReader(FileInfo File, string Delimiter = ",", int HeaderCount = 1)
            : base(File, Delimiter, HeaderCount)
        { }

        public TRecord GetRecord()
        {
            var record = default(TRecord);
            string[] raw;
            try
            {
                raw = this.WrappedCsvReader.Context.Record;
                record = this.WrappedCsvReader.GetRecord<TRecord>();
            }
            catch (Exception ex)
            {
                var newEx = new CsvReaderException(this.WrappedCsvReader.Context, ex);
                throw newEx;
            }
            return record;
        }

        public virtual async IAsyncEnumerable<TRecord> GetRecordsAsync()
        {
            _logger.Info($"Reading records from file: {this.SourceFile.Name}");
            while (await this.ReadAsync())
            {
                var record = this.GetRecord();
                yield return record;
            }
        }
    }
}
