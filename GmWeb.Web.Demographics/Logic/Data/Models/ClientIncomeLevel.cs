using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using GmWeb.Logic.Utility.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IL = GmWeb.Logic.Enums.IncomeLevel;

namespace GmWeb.Web.Demographics.Logic.DataModels
{
    public class ClientIncomeLevel :  ClientDetail
    {
        public static Expression<Func<ClientDetail, string>> CategorySelectorExpression => (ClientDetail x) =>
             !x.IncomeLevel.HasValue ? ""
             : x.IncomeLevel.Value == IL.ExtremeLow ? "Extremely Low"
             : x.IncomeLevel.Value == IL.Low ? "Low"
             : x.IncomeLevel.Value == IL.Moderate ? "Moderate"
             : x.IncomeLevel.Value == IL.AboveModerate ? "Above Moderate"
             : ""
        ;
        public string RawIncomeLevel { get; set; }

        public override string GetChartCategory() => this.RawIncomeLevel;

        private static Func<ClientDetail, string> _CategorySelector;
        public static Func<ClientDetail, string> CategorySelector => _CategorySelector ?? (_CategorySelector = CategorySelectorExpression.Compile());
    }
}