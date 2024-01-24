using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using System;
using System.Collections.Generic;

namespace GmWeb.Logic.Importing.Demographics
{
    public class MedianIncomeImporter : DemographicImporter<MedianIncomeRecord, MedianIncomeRecordMap>
    {
        public MedianIncomeImporter(IDemographicsContext cache, Category category) : base(cache, category) { }
        protected override IEnumerable<Func<MedianIncomeRecord, decimal?>> CategoryValueSelectors
        {
            get
            {
                yield return r => r.Value;
            }
        }
    }
}
