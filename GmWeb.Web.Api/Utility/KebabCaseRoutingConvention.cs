using GmWeb.Web.Api.Utility.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;
using System.Text.RegularExpressions;
using CaseExtensions;

namespace GmWeb.Web.Api.Utility
{
    public class KebabCaseActionConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            bool hasActionNameOverride = action.Attributes.OfType<ActionNameAttribute>().Any();
            if (hasActionNameOverride)
                return;
            action.ActionName = action.ActionName.ToKebabCase();
        }
    }
    public class KebabCaseControllerConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            bool hasControllerNameOverride = controller.Attributes.OfType<ControllerNameAttribute>().Any();
            if (hasControllerNameOverride)
                return;
            controller.ControllerName = controller.ControllerName.ToKebabCase();
        }
    }
}
