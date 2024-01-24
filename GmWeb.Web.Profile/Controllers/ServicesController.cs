using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GmWeb.Web.Profile.Models;
using GmWeb.Web.Profile.Models.Services;
using GmWeb.Logic.Utility.Extensions;
using Newtonsoft.Json;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace GmWeb.Web.Profile.Controllers
{
    public class ServicesController : BaseClientServicesController<ServicesViewModel>
    {
        // GET: Services
        public ActionResult Index()
        {
            var model = this.CreateViewModel();
            return View(model);
        }

        public ActionResult _ProcessStepGrid()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult GetProcessSteps([DataSourceRequest]DataSourceRequest request, int? ProcessTypeID, int? ClientCategoryID)
        {
            var rows = new List<ProcessStepRow>();
            if (!ClientCategoryID.HasValue || !ProcessTypeID.HasValue)
                return Json(new object[] { }, JsonRequestBehavior.AllowGet);
            var dc = this.DataComponent;
            var table = dc.GetProcessSteps(ProcessTypeID.Value, ClientCategoryID.Value, this.CurrentViewModel.ParentAgencyID);
            foreach(DataRow row in table.Rows)
            {
                rows.Add(new ProcessStepRow
                {
                    ActionText = row.ToString("Action"),
                    ActionUri = "http://wwww.example.com",
                    ProcessStepName = row.ToString("ProcessType"),
                    ProcessStepDescription = row.ToString("Description"),
                    EntityName = row.ToString("Entity"),
                    DocumentName = row.ToString("Document"),
                    DocumentRecordDate = row.ToDateTime("DateRecorded"),
                    Comments = row.ToString("DocumentComments"),
                    GroupNumber = row.ToInteger("GroupNumber")
                });
            }
            return Json(rows.ToDataSourceResult(request));
        }

        public JsonResult GetClientServices()
        {
            var dc = this.DataComponent;
            var data = dc.GetClientServices(this.CurrentViewModel.ClientID).ToDynamic();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcessTypes(int? ClientCategoryID)
        {
            if (!ClientCategoryID.HasValue)
                return Json(new object[] { }, JsonRequestBehavior.AllowGet);
            var dc = this.DataComponent;
            var data = dc.GetProcessTypes(ClientCategoryID.Value, this.CurrentViewModel.ParentAgencyID).ToDynamic();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _ProcessStepEditor()
        {
            var model = this.CreatePartialViewModel<ProcessStepModalViewModel>(x =>
            {
                x.ProcessTypeValue = "Test Type";
            });
            return PartialView(model);
        }
    }
}