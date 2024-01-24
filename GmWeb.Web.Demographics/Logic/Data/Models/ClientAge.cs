using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace GmWeb.Web.Demographics.Logic.DataModels
{
    public class ClientAge : ClientDetail
    {
        public override string GetChartCategory() => CategorySelector(this);
        public static Expression<Func<ClientDetail, string>> CategorySelectorExpression => x =>
        $"{((((ClientAge)x).Age.Value / 10) * 10)} to {(((ClientAge)x).Age.Value / 10) * 10 + 9}";

        private static Func<ClientDetail, string> _CategorySelector;
        public static Func<ClientDetail, string> CategorySelector => _CategorySelector ?? (_CategorySelector = CategorySelectorExpression.Compile());
    }
}