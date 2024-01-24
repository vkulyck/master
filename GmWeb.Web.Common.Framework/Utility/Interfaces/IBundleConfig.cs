using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace GmWeb.Web.Common.Utility
{
    public interface IBundleConfig
    {
        void RegisterBundles(BundleCollection bundles);
    }
}