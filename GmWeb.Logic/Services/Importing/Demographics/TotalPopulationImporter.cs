using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using System;
using System.Collections.Generic;

namespace GmWeb.Logic.Importing.Demographics
{
    public class TotalPopulationImporter : DemographicImporter<TotalPopulationRecord, TotalPopulationRecordMap>
    {
        public TotalPopulationImporter(IDemographicsContext cache, Category category) : base(cache, category) { }
        protected override IEnumerable<Func<TotalPopulationRecord, decimal?>> CategoryValueSelectors
        {
            get
            {
                yield return r => r.Value;
            }
        }
    }
}
