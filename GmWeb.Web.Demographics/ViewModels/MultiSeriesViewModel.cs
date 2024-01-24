using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using GmWeb.Web.Demographics.Logic.DataModels;
using MathNet.Numerics.LinearAlgebra.Double;
using Vec = MathNet.Numerics.LinearAlgebra.Double.DenseVector;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Math;
using GmWeb.Logic.Utility.Math.Coloring;

namespace GmWeb.Web.Demographics.ViewModels
{
    public class MultiSeriesViewModel
    {
        public struct DataIndex
        {
            public string Category;
            public string SeriesName;
            public int CategoryOrder;
            public DataIndex(string time, string name, int order)
            {
                this.Category = time;
                this.SeriesName = name;
                this.CategoryOrder = order;
            }
        }
        public class DataSeries
        {
            public List<double> Data { get; set; } = new List<double>();
            public List<string> Categories { get; set; } = new List<string>();
            public string SeriesName { get; set; }
        }

        public List<DataSeries> Series { get; set; } = new List<DataSeries>();
        public IEnumerable<string> SeriesNames { get; set; } 
        public IEnumerable<string> Categories { get; set; } 
        public string CategoryAxisTitle { get; set; }
        public string ValueAxisTitle { get; set; }
        public string Title { get; set; }
        public string Guid { get; set; } = GmWeb.Logic.Utility.Extensions.ModelExtensions.GenerateGuid();

        public static MultiSeriesViewModel FromClientData(IQueryable<ClientDetail> details, Expression<Func<ClientDetail, DataIndex>> indexSelector)
        {
            var data = details.GroupBy(indexSelector)
                .Select(x => new
                {
                    key = x.Key,
                    value = x.Count()
                })
                .AsEnumerable()
                .Select(x => new
                {
                    Order = x.key.CategoryOrder,
                    Category = x.key.Category,
                    SeriesName = x.key.SeriesName,
                    Data = x.value
                })
                .ToList()
            ;
            var model = new MultiSeriesViewModel();
            var categories = data.Select(x => new { x.Order, x.Category }).Distinct().OrderBy(x => x.Order).Select(x => x.Category).ToList();
            var seriesNames = data.Select(x => x.SeriesName).Distinct().ToList();
            foreach (var sname in seriesNames)
            {
                var ds = new DataSeries { SeriesName = sname };
                var seriesData = data.Where(x => x.SeriesName == sname).OrderBy(x => x.Order).ToList();
                foreach(var sdata in seriesData)
                {
                    ds.Categories.Add(sdata.Category);
                    ds.Data.Add(sdata.Data);
                }
                model.Series.Add(ds);
            }
            model.SeriesNames = seriesNames;
            model.Categories = categories;
            return model;
        }
    }
}