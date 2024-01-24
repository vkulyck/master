using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Utility.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GmWeb.Logic.Importing.Demographics
{
    public abstract class DemographicImporter<TRecord, TRecordMap> : CsvImporter<TRecord, TRecordMap, IDemographicsContext>
        where TRecord : DemographicRecord
        where TRecordMap : DemographicRecordMap<TRecord, TRecordMap>
    {
        public override int HeaderCount => 2;
        public virtual bool HasBins => false;
        public Category Category { get; private set; }
        public List<Bin> Bins { get; protected set; }
        protected MetadataImporter MetadataImporter { get; private set; }

        public DemographicImporter(IDemographicsContext cache, Category category) : base(cache, category.Dataset.SourcePath)
        {
            this.Category = category;
            this.Bins = this.Category.Bins.ToList();
        }

        public override async Task ImportAsync()
        {
            if (this.HasBins && this.Bins.Count == 0)
                await this.UpdateBinsAsync();

            var prevBinValues = await this.Cache.BinValues
                .Where(x => x.Bin.CategoryID == this.Category.CategoryID)
                .ToListAsync()
            ;
            await this.Cache.BulkDeleteAsync(prevBinValues);
            var binValues = new List<BinValue>();


            var prevCatValues = await this.Cache.CategoryValues
                .Where(x => x.CategoryID == this.Category.CategoryID)
                .ToListAsync()
            ;
            await this.Cache.BulkDeleteAsync(prevCatValues);
            var catValues = new List<CategoryValue>();

            await foreach (var record in this.IterateRecordsAsync())
            {
                // Some tract files have an aggregate row for the entire state with STATEFP as the entire GeoID
                if (record.DomesticGeoID.Length < 10)
                    continue;
                if (this.HasBins)
                    binValues.AddRange(this.CreateBinValues(record));
                else
                    catValues.AddRange(this.CreateCategoryValues(record));
            }
            if (binValues.Count > 0)
                await this.Cache.BulkInsertAsync(binValues);
            if (catValues.Count > 0)
                await this.Cache.BulkInsertAsync(catValues);
        }

        protected List<BinValue> CreateBinValues(TRecord record)
        {
            var values = new List<BinValue>();
            foreach (var bin in this.Bins)
            {
                decimal value = record.FieldValues[bin.ColumnID];
                var binValue = new BinValue
                {
                    BinID = bin.BinID,
                    Bin = bin,
                    TractID = record.DomesticGeoID,
                    Value = value
                };
                values.Add(binValue);
            }
            return values;
        }

        protected List<CategoryValue> CreateCategoryValues(TRecord record)
        {
            var values = new List<CategoryValue>();
            foreach (var selector in this.CategoryValueSelectors)
            {
                var catValue = new CategoryValue
                {
                    TractID = record.DomesticGeoID,
                    CategoryID = this.Category.CategoryID,
                    Category = this.Category,
                    Value = selector(record)
                };
                values.Add(catValue);
            }
            return values;
        }
        protected virtual IEnumerable<Func<TRecord, decimal?>> CategoryValueSelectors
        {
            get
            {
                yield break;
            }
        }

        protected virtual Bin CreateBinFromMetadata(MetadataRecord record) => throw new NotImplementedException();

        protected virtual async Task UpdateBinsAsync()
        {
            this.MetadataImporter = new MetadataImporter(
                this.Cache,
                this.Category,
                this.CreateBinFromMetadata
            );
            await this.MetadataImporter.ImportAsync();
            this.Bins.AddRange(this.MetadataImporter.Bins);
        }
    }
}
