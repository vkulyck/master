using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GmWeb.Web.Profile.Utility;
using GmWeb.Web.Profile.Models;
using GmWeb.Logic.Data.Context.Profile;
using DataModels = GmWeb.Logic.Data.Models.Waitlists;
using ViewModels = GmWeb.Web.Profile.Models.Waitlist;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Interfaces;
using Newtonsoft.Json;

namespace GmWeb.Web.Profile.Controllers
{
    public class WaitlistController : BaseClientServicesController<WaitlistViewModel>
    {
        // GET: Waitlist
        public ActionResult Index(int? FlowID, int? CategoryID = 264)
        {
            var model = this.CreateViewModel();
            using(var cache = new ProfileCache())
            {
                DataModels.Flow dbFlow;
                if (FlowID.HasValue)
                    dbFlow = cache.Flows.SingleOrDefault(x => x.FlowID == FlowID.Value);
                else dbFlow = cache.Flows.FirstOrDefault();
                dbFlow.PopulateReferences(cache.DataContext);
                var filter = new DataModels.ClientFilter(cache.DataContext);
                foreach (var dbData in dbFlow.Data)
                    dbData.ResolveLookup(cache.DataContext, CategoryID);
                foreach (var rs in dbFlow.Results)
                    rs.Data = filter.Evaluate(rs.Predicates, dbFlow);
                foreach (var step in dbFlow.Steps)
                    foreach (var q in step.Queries)
                        q.ResolveLookup(cache.DataContext, model.ClientID);
                model.Flow = this.Mapper.Map<ViewModels.Flow>(dbFlow);
            }
            if (model.Flow == null)
                throw new ArgumentException($"No waitlist flow configuration could be found with the requested ID: {FlowID}");
            return View(model);
        }

        // TODO: Convert this system of partials into react/angular controls
        [HttpPost]
        public ActionResult _FlowLinkEditor(ViewModels.FlowLink Model)
        {
            ViewBag.TitleSize = 36;
            ViewBag.IsCollapsible = true;
            return this.EditorContainer(Model);
        }

        [HttpPost]
        public ActionResult _FlowStepEditor(ViewModels.FlowStep Model)
        {
            ViewBag.TitleSize = 36;
            ViewBag.IsCollapsible = true;
            return this.EditorContainer(Model);
        }

        [HttpPost]
        public ActionResult _FlowEditor(ViewModels.Flow Model)
        {
            ViewBag.TitleSize = 36;
            ViewBag.IsCollapsible = true;
            return this.EditorContainer(Model);
        }

        [HttpPost]
        public ActionResult _FlowStepViewer(ViewModels.FlowStep Model)
        {
            return this.DisplayContainer(Model);
        }

        [HttpPost]
        public ActionResult SaveFlow(ViewModels.Flow Model)
        {
            using(var context = new ProfileCache())
            {
                var dbFlow = this.Mapper.Map<DataModels.Flow>(Model);
                context.Flows.Update(dbFlow);
                foreach(var step in dbFlow.Steps)
                {
                    var dbStep = this.Mapper.Map<DataModels.FlowStep>(step);
                    if (context.FlowSteps.Any(x => x.FlowStepID == step.FlowStepID))
                        context.FlowSteps.Update(dbStep);
                    else
                        context.FlowSteps.Add(dbStep);
                }
                context.Save();
            }
            return this.JsonSuccess();
        }
    }
}