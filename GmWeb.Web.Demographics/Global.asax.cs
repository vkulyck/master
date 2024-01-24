using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GmWeb.Web.Demographics
{
    public class MvcApplication : GmWeb.Web.Common.App_Start.BaseMvcApplication<FilterConfig, RouteConfig, BundleConfig>
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected override void Application_Start()
        {
            _logger.Info("Starting up demographics webapp.");
            base.Application_Start();
        }
    }
}
