using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using GmWeb.Web.Common.Controllers;

namespace GmWeb.Web.Common.Utility
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ActionAliasesAttribute : ActionNameSelectorAttribute
    {
        public IEnumerable<string> Aliases { get; private set; }
        public ActionAliasesAttribute(params string[] aliases)
        {
            this.Aliases = aliases.ToList();
        }

        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            var names = new List<string> { methodInfo.Name };
            names.AddRange(this.Aliases);
            return names.Any(x => String.Equals(actionName, x, StringComparison.OrdinalIgnoreCase));
        }
    }
}