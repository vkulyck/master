using Kendo.Mvc.Rendering;
using Kendo.Mvc.UI;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace GmWeb.Web.Scheduler.Models
{
    public class DemoServices
    {
        public static IEnumerable<ServiceDescriptor> GetServices()
        {
            yield return ServiceDescriptor.Scoped<ISchedulerEventService<TaskViewModel>, SchedulerTaskService>();
        }
    }
}
