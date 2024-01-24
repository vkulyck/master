using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using GmWeb.Common;
using GmWeb.Web.Common.Utility;

namespace GmWeb.Web.Common.Config
{
    public class CoreViewEngine : RazorViewEngine
    {
        public CoreViewEngine()
        {
            var extensions = new List<string> { "cshtml" };
            var paths = new List<string> {
                "~/Views/{1}/{0}",
                "~/Views/Shared/{0}",
                "~/bin/Views/{1}/{0}",
                "~/bin/Views/Shared/{0}",
                "~/bin/Views/{0}",
            };
            var views = new List<string>();
            foreach (var path in paths) 
            {
                foreach (var ext in extensions)
                {
                    var view = $"{path}.{ext}";
                    views.Add(view);
                }
            }

            this.MasterLocationFormats = views.ToArray();
            this.PartialViewLocationFormats = views.ToArray();
            this.ViewLocationFormats = views.ToArray();
        }
        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            foreach (var accountController in this.AccountControllers)
            {
                var reqController = controllerContext.GetControllerName();
                if (accountController == reqController)
                {
                    controllerContext.SetControllerName("Account");
                    var result = base.FindPartialView(controllerContext, partialViewName, useCache);
                    controllerContext.SetControllerName(reqController);
                    return result;
                }
            }
            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            foreach (var accountController in this.AccountControllers)
            {
                var reqController = controllerContext.GetControllerName();
                if (accountController == reqController)
                {
                    controllerContext.SetControllerName("Account");
                    var result = base.FindView(controllerContext, viewName, masterName, useCache);
                    controllerContext.SetControllerName(reqController);
                    return result;
                }
            }
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        protected IEnumerable<string> AccountControllers
        {
            get
            {
                foreach (var idType in AppIdentityConfig.IdentityTypes)
                    yield return $"{idType}Account";
            }
        }
    }
}
