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
    public class PieChartModel : IViewModel
    {
        public class ChartItem
        {
            public string category { get; set; }
            public int categoryId { get; set; }
            public double value { get; set; }
            public string color { get; set; }
        }

        public static PieChartModel FromClientData(IQueryable<ClientDetail> details, Expression<Func<ClientDetail,string>> categorySelector)
        {
            Dictionary<string, int> idMap = new Dictionary<string, int>();
            var model = new PieChartModel();
            string getColor(string category)
            {
                var id = idMap[category];
                if (id >= model.Colors.Count)
                {
                    int idMod = id % model.Colors.Count;
                    VColor cNext = new VColor(model.Colors[idMod]);
                    VColor cPrev;
                    if (idMod == 0)
                        cPrev = new VColor(model.Colors.Last());
                    else
                        cPrev = new VColor(model.Colors[idMod - 1]);
                    var midpoint = cPrev + (cNext - cPrev) / 2;
                    return midpoint.ToHex();
                }
                else
                    return model.Colors[id];
            }
            int getCatId(string category)
            {
                if (idMap.Count == 0)
                    idMap[category] = 0;
                else if (!idMap.ContainsKey(category))
                {
                    idMap[category] = idMap.Values.Max() + 1;
                }
                return idMap[category];
            }
            var data = details.GroupBy(categorySelector)
                .Select(x => new
                {
                    category = x.Key,
                    value = x.Count()
                })
                .AsEnumerable()
                .Select(x => new ChartItem
                {
                    category = x.category,
                    categoryId = getCatId(x.category),
                    value = x.value,
                    color = getColor(x.category)
                })
                .ToList()
            ;
            model.SetItems(data);
            return model;
        }

        public IEnumerable<ChartItem> Items { get; private set; }

        public string Title { get; set; }
        public string Guid { get; set; } = GmWeb.Logic.Utility.Extensions.ModelExtensions.GenerateGuid();

        public PieChartModel() { }
        public PieChartModel(IEnumerable<ChartItem> items)
        {
            this.SetItems(items);
        }

        public void SetItems(IEnumerable<ChartItem> items)
        {
            this.Items = items.ToList();
        }

        public List<string> Colors { get; } = new List<string>
        {
            "#003f5c",
            "#2f4b7c",
            "#665191",
            "#a05195",
            "#d45087",
            "#f95d6a",
            "#ff7c43",
            "#ffa600"
        };
    }
}
