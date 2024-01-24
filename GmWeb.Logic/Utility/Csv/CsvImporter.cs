using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsyncContext = Nito.AsyncEx.AsyncContext;

namespace GmWeb.Logic.Utility.Csv
{
    public abstract class CsvImporter<TRecord, TRecordMap, TContext> : IDataImporter, IConditionallyDisposable
        where TRecord : CsvRecord
        where TRecordMap : CsvRecordMap<TRecord, TRecordMap>
        where TContext : IBaseDataContext
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TContext Cache { get; set; }
        public virtual string Name => Regex.Replace(this.GetType().Name, @"(.*)Importer(?:$|<)", "$1");
        public IReadOnlyList<string[]> HeaderRecords { get; private set; }
        public IReadOnlyDictionary<string, int> HeaderIndexes { get; private set; }
        public virtual int HeaderCount { get; } = 1;
        protected FileInfo SourceFile { get; private set; }
        public CsvImporter(TContext cache, FileInfo source)
        {
            this.Cache = cache;
            this.EnableDispose = false;
            this.SourceFile = source;
        }
        public CsvImporter(TContext cache, string sourcePath) : this(cache, new FileInfo(sourcePath)) { }

        public abstract Task ImportAsync();

        public virtual async IAsyncEnumerable<TRecord> IterateRecordsAsync()
        {
            _logger.Info($"Reading records from file: {this.SourceFile.Name}");
            using (var reader = new CsvMapReader<TRecord, TRecordMap>(File: this.SourceFile, HeaderCount: this.HeaderCount))
            {
                this.HeaderRecords = reader.HeaderRecords.ToList();
                this.HeaderIndexes = reader.HeaderIndexes.ToDictionary(x => x.Key, x => x.Value);
                await foreach(var record in reader.GetRecordsAsync())
                {
                    yield return record;
                }
            }
        }

        public bool EnableDispose { get; private set; } = true;
        public virtual void Dispose()
        {
            if (!this.EnableDispose)
                return;
            this.Cache.Dispose();
        }
    }
}
