using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using GmWeb.Web.Demographics.Helpers;
using GmWeb.Web.Common.Identity;
using System.IO;
using System.Text;
using System.Web.UI;
using GmWeb.Web.Common;
using GmWeb.Web.Common.Controllers;
using GmWeb.Web.Common.Models;

namespace GmWeb.Web.Demographics.Controllers
{
    public class BaseClientServicesController : BaseClientServicesController<BasePageViewModel> { }
    public class BaseClientServicesController<T> : BaseController<ClientProfileUser>
        where T : BasePageViewModel, new()
    {
        public AutoMapper.IMapper Mapper => ClientServicesMapping.Mapper;
        protected T CurrentViewModel
        {
            get
            {
                var model = this.Session["Model"] as T;
                if (model == null)
                    throw new Exception("No view model could be found in the session.");
                return model;
            }
            private set
            {
                this.Session["Model"] = value;
            }
        }

        protected T CreateViewModel() => CreateViewModel(null);

        protected T CreateViewModel(Action<T> initializer)
        {
            var model = this.CreatePartialViewModel<T>(initializer);
            this.CurrentViewModel = model;
            return model;
        }

        protected U CreatePartialViewModel<U>(Action<U> initializer) where U : BasePageViewModel, new()
        {
            var model = new U();
            var user = null as ClientProfileUser; // TODO: FIX THIS this.CurrentUser;
            model.ClientID = user.ClientID;
            model.AgencyName = user.AgencyName;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.FullName = $"{user.FirstName} {user.LastName}";
            model.Email = user.Email;
            model.AgencyID = user.AgencyID;
            model.AgencyIDParent = user.ParentAgencyID;
            if(initializer != null)
                initializer(model);
            return model;
        }
    }
}
