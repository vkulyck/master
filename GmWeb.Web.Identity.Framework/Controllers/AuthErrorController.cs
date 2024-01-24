using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GmWeb.Web.Common.Controllers;
using GmWeb.Web.Common.Models;
using GmWeb.Web.Identity.Models;

namespace GmWeb.Web.Identity.Controllers
{
    public class AuthErrorController : BaseController
    {
        [HttpGet]
        public ActionResult ForbiddenRegistration()
        {
            return this.View();
        }        
        [HttpGet]
        public ActionResult UnauthorizedRegistration()
        {
            return this.View();
        }
    }
}