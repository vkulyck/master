using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Utility.Csv;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GmWeb.Logic.Importing.Demographics
{
    public class MetadataImporter : CsvImporter<MetadataRecord, MetadataRecordMap, IDemographicsContext>
    {
        public override int HeaderCount => 0;
        private Dictionary<string, string> _IdToDescription { get; } = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> IdToDescription => this._IdToDescription;
        private Dictionary<string, string> _DescriptionToId { get; } = new Dictionary<string, string>();
        public IReadOnlyDictionary<string, string> DescriptionToId => this._DescriptionToId;

        public Category Category { get; private set; }
        public Func<MetadataRecord, Bin> Converter { get; private set; }
        protected List<Bin> _Bins { get; } = new List<Bin>();
        public IEnumerable<Bin> Bins => this._Bins;

        public MetadataImporter(
            IDemographicsContext Cache,
            Category Category,
            Func<MetadataRecord, Bin> Converter
        ) : base(Cache, Category.Dataset.MetadataPath)
        {
            this.Category = Category;
            this.Converter = Converter;
        }

        public override async Task ImportAsync()
        {
            await foreach (var record in this.IterateRecordsAsync())
            {
                var bin = this.Converter(record);
                if (bin == null)
                    continue;
                this._Bins.Add(bin);
                this.Cache.Save();
            }
        }
    }
}
