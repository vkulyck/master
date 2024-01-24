using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GmWeb.Web.Common.Utility
{
    public interface IFilterConfig
    {
        void RegisterGlobalFilters(GlobalFilterCollection filters);
    }
}