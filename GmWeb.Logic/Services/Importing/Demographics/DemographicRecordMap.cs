using GmWeb.Logic.Utility.Csv;
using GmWeb.Logic.Utility.Extensions.Expressions;
using GmWeb.Logic.Utility.Extensions.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;

namespace GmWeb.Logic.Importing.Demographics
{
    public abstract class DemographicRecordMap<TRecord, TRecordMap> : CsvRecordMap<TRecord, TRecordMap>
        where TRecord : DemographicRecord
        where TRecordMap : DemographicRecordMap<TRecord, TRecordMap>
    {
        public virtual bool EnableFieldValueMap => false;
        public virtual int? FieldValueMapEndIndex => null;
        public DemographicRecordMap()
        {
            AutoMap();
            string pLocalId = @"(?<LocalGeoID>(?:\d+)(?:\.\d+)?)";
            string pCounty = @"(?<County>\w+) County";
            string pState = @"(?<State>\w+)";
            string pLabel = $@"^\s*Census Tract {pLocalId}, {pCounty}, {pState}\s*$";
            MemberMap<TRecord, string> GeoMap(Expression<Func<TRecord, string>> x)
                => this.RegexMap(x, new Regex(pLabel), this.RowField("GEO.display-label"));
            GeoMap(x => x.LocalGeoID);
            GeoMap(x => x.County);
            GeoMap(x => x.State);
            if (this.EnableFieldValueMap)
                this.MapFieldValues();
        }

        protected void MapFieldValues()
        {
            Map(x => x.FieldValues).ConvertUsing((IReaderRow row) =>
            {
                var headers = row.Context.HeaderRecord.Skip(3).ToList(); // The first 3 headers are ids/labels
                var dict = new Dictionary<string, decimal>();
                int maxIndex = this.FieldValueMapEndIndex.HasValue ? this.FieldValueMapEndIndex.Value : headers.Count - 1;
                for (int i = 0; i <= maxIndex; i++)
                {
                    var header = headers[i];
                    if (row.TryGetField<decimal>(header, out decimal fieldValue))
                    {
                        dict[header] = fieldValue;
                    }
                }
                return dict;
            });
        }

        protected Func<IReaderRow, string> RowField(string fieldName) => x => x.GetField(fieldName);

        protected MemberMap<TRecord, string> RegexMap(Expression<Func<TRecord, string>> expression, Regex parser, Func<IReaderRow, string> rowTransform)
        {
            var property = expression.GetProperty();
            return Map(expression).ConvertUsing(row =>
            {
                string raw = rowTransform(row);
                var match = parser.Match(raw);
                if (!match.Success)
                    return null;
                return match.Groups[property.Name].Value;
            });
        }

        public override TRecordMap Init()
        {
            var numericMaps = this.MemberMaps.Where(x => x.Data.Member.IsMemberNullableNumeric()).ToList();
            foreach (var nm in numericMaps)
            {
                nm.TypeConverterOption.NullValues("-", "N");
            }
            return (TRecordMap)this;
        }
    }
}
