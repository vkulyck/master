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
using GmWeb.Web.Profile.Models;
using GmWeb.Web.Profile.Models.Shared;
using GmWeb.Web.Profile.Utility;
using GmWeb.Web.Common.Identity;
using System.IO;
using System.Text;
using System.Web.UI;
using GmWeb.Web.Common;
using GmWeb.Web.Common.Controllers;
using GmWeb.Web.Common.Models;

namespace GmWeb.Web.Profile.Controllers
{
    [Authorize]
    public class BaseClientServicesController<TModel> : BaseController<GmIdentity>
        where TModel : BasePageViewModel, new()
    {
        protected TModel CurrentViewModel
        {
            get
            {
                var model = this.Session["Model"] as TModel;
                if (model == null)
                    throw new Exception("No view model could be found in the session.");
                return model;
            }
            private set
            {
                this.Session["Model"] = value;
            }
        }

        protected TModel CreateViewModel() => CreateViewModel(null);

        protected TModel CreateViewModel(Action<TModel> initializer)
        {
            var model = this.CreatePartialViewModel<TModel>(initializer);
            this.CurrentViewModel = model;
            return model;
        }

        protected U CreatePartialViewModel<U>(Action<U> initializer) where U : BasePageViewModel, new()
        {
            var model = new U();
            var user = this.CurrentUser;
            model.ClientID = user.UserID;
            model.AgencyName = user.Account.AgencyID.ToString();
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.FullName = $"{user.FirstName} {user.LastName}";
            model.Email = user.Email;
            model.AgencyID = user.AgencyID;
            model.AgencyIDParent = user.Account.AgencyID ?? 0;
            if(initializer != null)
                initializer(model);
            return model;
        }
    }
}
