﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GmWeb.Web.Profile.Models;

namespace GmWeb.Web.Profile.Controllers
{
    public class VolunteeringController : BaseClientServicesController<VolunteeringViewModel>
    {
        // GET: Volunteering
        public ActionResult Index()
        {
            var model = this.CreateViewModel();
            return View(model);
        }
    }
}