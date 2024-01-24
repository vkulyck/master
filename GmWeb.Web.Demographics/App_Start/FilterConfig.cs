using System.Web;
using System.Web.Mvc;
using GmWeb.Web.Common.Utility;

namespace GmWeb.Web.Demographics
{
    public class FilterConfig : IFilterConfig
    {
        public void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SessionExpirationFilter());
        }
    }
}
