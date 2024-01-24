using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Linq;

namespace GmWeb.Web.Api.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ControllerNameAttribute : Attribute
    {
        public string Name { get; }

        public ControllerNameAttribute(string name)
        {
            this.Name = name;
        }

        public class Convention : IControllerModelConvention
        {
            public void Apply(ControllerModel controller)
            {
                var controllerNameAttribute = controller.Attributes.OfType<ControllerNameAttribute>().SingleOrDefault();
                if (controllerNameAttribute != null)
                {
                    controller.ControllerName = controllerNameAttribute.Name;
                }
            }
        }
    }
}
