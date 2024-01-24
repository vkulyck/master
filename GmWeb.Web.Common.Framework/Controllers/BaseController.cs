using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Common.Models.Shared;
using System.IO;
using System.Text;
using System.Web.UI;
using GmWeb.Common;
using GmWeb.Web.Common.Identity;
using GmWeb.Logic.Interfaces;
using System.Configuration;

namespace GmWeb.Web.Common.Controllers
{
    [ValidatePostForgery]
    public abstract class BaseController : Controller
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string DefaultController => ConfigurationManager.AppSettings[$"DefaultController"]?.ToString() ?? "Home";
        public static string DefaultUrl => $"~/{DefaultController}/";
        protected string ControllerName => this.RouteVariable("controller");
        protected string ActionName => this.RouteVariable("action");
        public string RequestBrowser => $"{this.HttpContext?.Request.Browser.Platform} {this.HttpContext?.Request.Browser.Browser} {this.HttpContext?.Request.Browser.Version}";
        public virtual IDisposableMapper Mapper => OwinContext.Get<IDisposableMapper>();
        public virtual bool IsAuthenticated => this.User.Identity.IsAuthenticated;
        protected virtual IOwinContext OwinContext => HttpContext.GetOwinContext();
        protected string RouteVariable(string variableName) => this.ControllerContext.RouteData.Values["accountType"]?.ToString();
        protected T RouteVariable<T>(string variableName) where T : struct
            => Enum.TryParse<T>(this.RouteVariable(variableName), out T result) ? result : default(T)
        ;
        protected DataComponent DataComponent { get; private set; }

        protected bool HasModelErrors => ModelState.Values.Any(x => x.Errors.Count > 0);

        public BaseController()
        {
            this.DataComponent = new DataComponent
            {
                BrowserSelector = () => this.RequestBrowser,
                SessionDataSelector = () => this.Session.ToDictionary()
            };
        }

        protected HttpCookie GetCookie(string name, bool createMissing = true, string createValue = null)
        {
            var cookie = Request.Cookies[name];
            if (cookie == null)
            {
                cookie = new HttpCookie(name, createValue);
                Request.Cookies.Add(cookie);
            }
            return cookie;
        }

        public PartialViewResult EditorContainer<TModel>(TModel Model) where TModel : IEditableViewModel
        {
            return PartialView("EditorTemplates/FieldContainer", Model);
        }

        public PartialViewResult DisplayContainer<TModel>(TModel Model) where TModel : IEditableViewModel
        {
            return PartialView("DisplayTemplates/FieldContainer", Model);
        }

        protected Task<ViewResult> ErrorViewAsync(object model, string message)
        {
            ViewBag.StatusMessage = message;
            ViewBag.StatusType = "danger";
            return Task.FromResult(base.View(model));
        }

        protected ViewResult StatusView(object model)
        {
            if (this.HasModelErrors)
            {
                if (ViewBag.StatusMessage == null)
                    ViewBag.StatusMessage = "An error occured during the last request.";
                ViewBag.StatusType = "danger";
            }
            else
            {
                if (ViewBag.StatusMessage == null)
                    ViewBag.StatusMessage = "";
                ViewBag.StatusType = "success";
            }
            return base.View(model);
        }

        protected new ViewResult View(object model) => StatusView(model);
        protected async Task<ViewResult> ViewAsync() => await Task.FromResult(View());
        protected async Task<ViewResult> ViewAsync(object model) => await Task.FromResult(View(model));

        protected bool IsEmpty(string value) => string.IsNullOrWhiteSpace(value);
        protected bool IsNotEmpty(string value) => !this.IsEmpty(value);
        protected bool AllEmpty(params string[] values) => !values.Any(x => this.IsNotEmpty(x));
        protected bool AnyEmpty(params string[] values) => values.Any(x => this.IsEmpty(x));

        protected virtual ActionResult UnauthorizedResult()
        {
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
        }
        protected virtual ActionResult ErrorResult(string statusDescription)
        {
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, statusDescription);
        }

        protected Task<RedirectResult> RedirectOrDefaultAsync(string returnUrl)
        {
            return Task.FromResult(this.RedirectOrDefault(returnUrl));
        }
        protected RedirectResult RedirectOrDefault(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToDefault();
            return Redirect(returnUrl);
        }

        protected RedirectResult RedirectToDefault() => Redirect(DefaultUrl);

        protected new JsonResult Json(object data, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                JsonRequestBehavior = behavior
            };
        }

        protected JsonResult JsonSuccess()
        {
            return Json(new { success = true });
        }

        protected HttpStatusCodeResult JsonFailure(string msg, JsonRequestBehavior Behavior = JsonRequestBehavior.DenyGet)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, msg);
        }

        protected ActionResult JsonFailure(Exception ex, JsonRequestBehavior Behavior = JsonRequestBehavior.DenyGet)
        {
            return this.JsonFailure(ex.Message, Behavior);
        }

        protected string RenderPartialView(string viewName)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter tw = new HtmlTextWriter(sw))
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, tw);
                }
            }
            var rendered = sb.ToString();
            return rendered;
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }

    public class BaseController<TUser> : BaseController
        where TUser : GmIdentity, new()
    {
        protected IAuthenticationManager AuthenticationManager => OwinContext.Authentication;
        protected virtual GmManager<TUser> UserManager => OwinContext.Get<GmManager<TUser>>();
        protected GmSignInManager<TUser> SignInManager => OwinContext.Get<GmSignInManager<TUser>>();
        private TUser _user;

        private string CurrentUserId { get; set; }
        public TUser CurrentUser
        {
            get
            {
                if (_user == null || CurrentUserId != User?.Identity?.GetUserId())
                {
                    CurrentUserId = User?.Identity?.GetUserId();
                    if (string.IsNullOrWhiteSpace(CurrentUserId))
                        return null;
                    _user = this.UserManager.FindById(CurrentUserId);
                }
                return _user;
            }
            protected set { _user = value; }
        }

        protected async Task RefreshSignInAsync(TUser model, bool isPersistent = false, string returnUrl = null)
        {
            await this.SignInManager.RefreshSignInAsync(model, isPersistent, returnUrl);
            this.CurrentUser = null;
        }
        protected async Task RefreshSignInAsync(string userId, bool isPersistent = false, string returnUrl = null)
        {
            await this.SignInManager.RefreshSignInAsync(userId, isPersistent, returnUrl);
            this.CurrentUser = null;
        }
    }
}
