using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GmWeb.Web.Scheduler.Models;
using Ical.Net.CalendarComponents;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GmWeb.Web.Scheduler.Controllers
{
    public partial class SchedulerController : Controller
    {
        private ISchedulerEventService<TaskViewModel> taskService;

        public SchedulerController(ISchedulerEventService<TaskViewModel> schedulerTaskService )
        {
            taskService = schedulerTaskService;
        }

        [HttpGet]
        public virtual JsonResult ReadImportedEvents([DataSourceRequest] DataSourceRequest request)
        {
            return Json(((SchedulerTaskService)taskService).ReadImportedEvents().ToDataSourceResult(request)/*, JsonRequestBehavior.AllowGet*/);
        }

        public virtual JsonResult Read_Attendees([DataSourceRequest] DataSourceRequest request)
        {
            return Json(((SchedulerTaskService)taskService).GetAttendees().ToDataSourceResult(request)/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult Create_Attendees([DataSourceRequest] DataSourceRequest request, Attendee attendee)
        {
            if (ModelState.IsValid)
            {
                ((SchedulerTaskService)taskService).InsertAttendee(attendee);
            }

            return Json(new[] { attendee }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update_Attendees([DataSourceRequest] DataSourceRequest request, Attendee attendee)
        {
            if (ModelState.IsValid)
            {
                ((SchedulerTaskService)taskService).UpdateAttendee(attendee, ModelState);
            }

            return Json(new[] { attendee }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Destroy_Attendees([DataSourceRequest] DataSourceRequest request, Attendee attendee)
        {
            if (ModelState.IsValid)
            {
                ((SchedulerTaskService)taskService).DestroyAttendee(attendee, ModelState);
            }

            return Json(new[] { attendee }.ToDataSourceResult(request, ModelState));
        }

        //[Demo]
        public IActionResult Index()
        {
            return View();
        }

        public virtual JsonResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(taskService.GetAll().ToDataSourceResult(request));
        }

        public virtual JsonResult Destroy([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                taskService.Delete(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Create([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                taskService.Insert(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Update([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            //example custom validation:
            //if (task.Start.Hour < 8 || task.Start.Hour > 22)
            //{
            //    ModelState.AddModelError("start", "Start date must be in working hours (8h - 22h)");
            //}

            if (ModelState.IsValid)
            {
                taskService.Update(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public IActionResult SaveData()
        {
            ((SchedulerTaskService)taskService).SaveChanges();
            return Ok();
        }

        [HttpGet]
        public JsonResult ExportData()
        {
            string strCalData = ((SchedulerTaskService)taskService).ExportICalData();
            return Json(strCalData);
        }

        [HttpPost]
        public IActionResult ImportFilteredEvents(IEnumerable<string> arrayOfValues/*string strEvents*/)
        {
            ((SchedulerTaskService)taskService).ImportFilteredEvents(arrayOfValues);
            return Ok();
        }

        [HttpPost]
        public IActionResult ImportEvents(string strEvents, string strIcalFilePath)
        {
            ((SchedulerTaskService)taskService).ImportEvents(strEvents, strIcalFilePath);
            return Ok();
        }

        [HttpPost]
        public IActionResult ImportData(string strIcalData, string strIcalFilePath)
        {
            ((SchedulerTaskService)taskService).ImportICalData(strIcalData, strIcalFilePath);
            return Ok();
        }
    }
}

