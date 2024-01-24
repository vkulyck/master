using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace GmWeb.Web.Api.Utility.Attributes
{
    public class ActionNameConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            var actionNameAttribute = action.Attributes.OfType<ActionNameAttribute>().SingleOrDefault();
            if (actionNameAttribute != null)
            {
                action.ActionName = actionNameAttribute.Name;
            }
        }
    }
}
