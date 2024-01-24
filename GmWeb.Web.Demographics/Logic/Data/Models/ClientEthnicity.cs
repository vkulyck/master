using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using GmWeb.Logic.Interfaces;
using IL = GmWeb.Logic.Enums.IncomeLevel;

namespace GmWeb.Web.Demographics.Logic.DataModels
{
    public class ClientEthnicity : ClientDetail
    {
        public static Expression<Func<ClientDetail, string>> CategorySelectorExpression => (ClientDetail x) => string.IsNullOrWhiteSpace(x.Ethnicity) ? "Unspecified" : x.Ethnicity;
        private static Func<ClientDetail, string> _CategorySelector;
        public static Func<ClientDetail, string> CategorySelector => _CategorySelector ?? (_CategorySelector = CategorySelectorExpression.Compile());
        public override string GetChartCategory() => this.Ethnicity;
    }
}