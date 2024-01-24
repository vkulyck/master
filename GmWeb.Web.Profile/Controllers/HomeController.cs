using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Web.Profile.Models;
using GmWeb.Web.Profile.Models.Home;

namespace GmWeb.Web.Profile.Controllers
{
    [Authorize]
    public class HomeController : BaseClientServicesController<HomeViewModel>
    {
        public ActionResult Index()
        {
            var model = this.CreateViewModel(m =>
            {
                m.Birthday = "7/8/1965";
                m.Ethnicity = "Black or African American";
                m.LanguageIDSecondary = "French";
                m.CulturalIdentityID = "United States or American";
                m.LanguageIDPrimary = "English";
                m.Phone = "415-555-1234";
                m.Address = "123 Market St.";
                m.AddressLine2 = "";
                m.Gender = "Male";
            });

            return View(model);
        }

        public ActionResult GetPersonCategory(int CategoryID, int ClientCategoryID)
        {
            var result = this.DataComponent.GetPersonCategory(CategoryID).RowsToModels<PersonCategory>().SingleOrDefault();
            result.IsActionNeeded = this.DataComponent.GetProcessTypes(ClientCategoryID, this.CurrentViewModel.AgencyIDParent).Rows.Count > 0;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClientServicesDates()
        {
            var results = this.DataComponent.GetClientServicesDates(this.CurrentViewModel.ClientID).RowsToModels<ClientService>();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFamilyMembers()
        {
            var results = this.DataComponent.GetFamilyMembers(this.CurrentViewModel.ClientID).RowsToModels<FamilyMember>();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveClientCategoryDate(int ClientCategoryDateID, int ClientCategoryID)
        {
            var results = this.DataComponent.RemoveClientCategoryDate(ClientCategoryDateID, ClientCategoryID).RowsToModels<FamilyMember>();
            return Json(new { Success = true }, JsonRequestBehavior.DenyGet);
        }
    }
}
