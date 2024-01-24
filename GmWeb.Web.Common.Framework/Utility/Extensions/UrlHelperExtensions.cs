using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GmWeb.Web.Common.Utility
{
    public static class UrlHelperExtensions
    {
        public static string ContentPath(this UrlHelper url, string content)
        {
            return url.Content($"~/Content/{content}");
        }

        public static string ImagePath(this UrlHelper url, string image)
        {
            return url.ContentPath($"images/{image}");
        }

        public static string ScriptPath(this UrlHelper url, string script)
        {
            return url.Content($"~/Scripts/{script}");
        }
    }
}