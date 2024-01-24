using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using System;
using System.Collections.Generic;

namespace GmWeb.Logic.Importing.Demographics
{
    public class MeanIncomeImporter : DemographicImporter<MeanIncomeRecord, MeanIncomeRecordMap>
    {
        public MeanIncomeImporter(IDemographicsContext cache, Category category) : base(cache, category) { }
        protected override IEnumerable<Func<MeanIncomeRecord, decimal?>> CategoryValueSelectors
        {
            get
            {
                yield return r => r.Value;
            }
        }
    }
}
