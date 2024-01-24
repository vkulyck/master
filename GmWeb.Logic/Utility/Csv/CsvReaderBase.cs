using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Csv
{
    public abstract class CsvReaderBase : IDisposable
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected FileInfo SourceFile { get; private set; }
        protected FileInfo WorkingFile { get; private set; }
        protected StreamReader StreamReader { get; }
        protected CsvHelper.CsvReader WrappedCsvReader { get; }

        protected bool IsHeaderNeeded { get; private set; } = false;
        public List<string[]> HeaderRecords { get; } = new List<string[]>();
        public Dictionary<string, int> HeaderIndexes { get; } = new Dictionary<string, int>();
        public int HeaderCount { get; private set; } = 1;

        public CsvReaderBase(FileInfo File, string Delimiter, int HeaderCount)
        {
            this.SourceFile = File;
            this.WorkingFile = File;
            this.StreamReader = new StreamReader(new FileStream(this.WorkingFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            this.WrappedCsvReader = new CsvHelper.CsvReader(this.StreamReader);

            // Allow the reader to skip unused fields.
            this.WrappedCsvReader.Configuration.HeaderValidated = null;
            this.WrappedCsvReader.Configuration.MissingFieldFound = null;

            this.HeaderCount = HeaderCount;
            this.WrappedCsvReader.Configuration.HasHeaderRecord = HeaderCount > 0;
            this.IsHeaderNeeded = HeaderCount > 0;
            this.WrappedCsvReader.Configuration.Delimiter = Delimiter;
        }

        /// <summary>
        /// Gets the configured number of header lines, advancing the file pointer as many lines as necessary. 
        /// Upon completion, the file pointer is left at a new, unprocessed, non-header record line.
        /// </summary>
        protected async Task<bool> ReadHeaderAsync()
        {
            if (!this.IsHeaderNeeded)
                return true;
            this.IsHeaderNeeded = false;
            for (int i = 0; i < this.HeaderCount; i++)
            {
                var result = await this.WrappedCsvReader.ReadAsync();
                if (result)
                {
                    string[] nextHeader;
                    if (i == 0)
                    {
                        this.WrappedCsvReader.ReadHeader();
                        nextHeader = this.WrappedCsvReader.Context.HeaderRecord;
                    }
                    else
                        nextHeader = this.WrappedCsvReader.Context.Record;
                    this.HeaderRecords.Add(nextHeader);
                    for (int j = 0; j < nextHeader.Length; j++)
                    {
                        string h = nextHeader[j];
                        this.HeaderIndexes[h] = j;
                    }
                }
                else
                    return false;
            }
            return true;
        }

        public async Task<bool> ReadAsync()
        {
            try
            {
                bool result = await this.ReadHeaderAsync();
                if (!result)
                    return false;
                result = await this.WrappedCsvReader.ReadAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error reading line {this.WrappedCsvReader.Context.RawRow} from file: '{this.WorkingFile}'", ex);
                _logger.Info($"Previous record: {this.WrappedCsvReader.Context.Record}");
                throw;
            }
        }

        public T GetFieldValue<T>(string fieldName) => this.WrappedCsvReader.GetField<T>(fieldName);
        public T GetFieldValue<T>(int index) => this.WrappedCsvReader.GetField<T>(index);

        public void Dispose()
        {
            if (this.StreamReader != null)
                this.StreamReader.Dispose();
            if (this.WrappedCsvReader != null)
                this.WrappedCsvReader.Dispose();
        }
    }
}
