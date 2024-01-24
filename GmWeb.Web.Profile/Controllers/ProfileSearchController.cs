using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GmWeb.Web.Profile.Models;
using GmWeb.Web.Profile.Models.Shared;
using GmWeb.Web.Profile.Models.ProfileSearch;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Web.Common.Utility;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace GmWeb.Web.Profile.Controllers
{
    public class ProfileSearchController : BaseClientServicesController<ProfileSearchViewModel>
    {
        [DefaultRedirectFilter]
        public ActionResult Index(SearchType? SearchType = SearchType.Profiles, int? ClientDataTypeID = 801)
        {
            var dc = this.DataComponent;
            var clientDataType = dc.GetClientDataType(ClientDataTypeID.Value).RowsToModels<ClientDataType>().Single();
            var model = this.CreateViewModel(m =>
            {
                m.Description = clientDataType.Description;
                m.Category = clientDataType.Category;
                m.EnablePrevButton = clientDataType.PrevClientDataTypeID.HasValue;
                m.EnableNextButton = clientDataType.NextClientDataTypeID.HasValue;
                m.ClientDataTypeID = ClientDataTypeID.Value;
                m.SearchType = SearchType.Value;
            });
            return View(model);
        }

        public ActionResult GetConsentItems()
        {
            var models = new List<AgreementItem>();
            var model = new AgreementItem
            {
                 Description = "agree to this thing, please",
                 LegalVerbiage = "if you agree to this thing we own your soul"
            };
            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetKeywords()
        {
            var dc = this.DataComponent;
            var keywords = dc.GetServiceKeywords().RowsToStrings();
            return Json(keywords, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SearchKeywordCategories(List<string> Keywords)
        {
            var dc = this.DataComponent;
            var categoryIDs = dc.GetKeywordCategoryList(Keywords);
            var table = dc.GetCategoriesFromList(categoryIDs);
            // TODO: What should this return?
            return Json(new List<string> { categoryIDs });
        }

        public ActionResult GetClientDataTypes()
        {
            var dc = this.DataComponent;
            var models = dc.GetClientDataTypes().RowsToModels<ClientDataType>();
            return Json(models, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetProfiles([DataSourceRequest]DataSourceRequest request, int ClientDataTypeID)
        {
            var dc = this.DataComponent;
            var models = dc.GetProfileSetupData("CLI", this.CurrentViewModel.ClientID, ClientDataTypeID).RowsToModels<ProfileRow>();
            return Json(models.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult QueryServices([DataSourceRequest]DataSourceRequest request, List<string> Keywords, List<ProfileRow> Profiles, List<ServiceRow> SelectedServices, SearchType SearchType, double Threshold = 0.75)
        {
            if (SearchType == SearchType.Keywords)
            {
                return this.GetKeywordSearchResults(request, Keywords);
            }
            else if (SearchType == SearchType.Profiles)
            {
                return this.GetProfileSearchResults(request, Profiles, Threshold);
            }
            else if (SearchType == SearchType.Qualifying)
            {
                return this.GetQualifyingServices(request, SelectedServices);
            }
            return this.JsonFailure("Invalid search type requested.");
        }

        public ActionResult GetProfileSearchResults([DataSourceRequest]DataSourceRequest request, List<ProfileRow> Profiles, double Threshold)
        {
            foreach (var profile in Profiles)
            {
                if (profile.IsChecked)
                {
                    // TODO: Is this necessary? It shouldn't be possible to retrieve an invalid profile via GetProfiles
                    if (this.DataComponent.GetClientProfile(this.CurrentViewModel.ClientID, profile.ProfileName).Rows.Count == 0)
                        return this.JsonFailure($"{profile.ProfileName} profile is not configured for this client.");
                }
            }

            var names = Profiles
                .Where(x => x.IsChecked)
                .Select(x => x.ProfileName)
                .ToList()
            ;
            var profileList = string.Join(",", names);
            string categoryIDList = this.DataComponent.GetClientCategoryCompareList(this.CurrentViewModel.ClientID, profileList, Threshold);
            return GetCategoryServices(request, categoryIDList);
        }

        [HttpPost]
        public ActionResult GetKeywordSearchResults([DataSourceRequest]DataSourceRequest request, List<string> Keywords)
        {
            if (Keywords.Count == 0)
                return this.JsonFailure("No keywords specified for search request.");
            string categoryIDList = this.DataComponent.GetKeywordCategoryList(Keywords);
            return GetCategoryServices(request, categoryIDList);
        }

        public ActionResult GetCategoryServices([DataSourceRequest]DataSourceRequest request, string CategoryIDList)
        {
            var table = this.DataComponent.GetCategoriesFromList(CategoryIDList);
            var models = table.RowsToModels<ServiceRow>();
            return Json(models.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetQualifyingServices([DataSourceRequest]DataSourceRequest request, List<ServiceRow> SelectedServices)
        {
            var categoryIDs = SelectedServices
                .Where(x => x.IsChecked)
                .Select(x => x.CategoryID)
                .ToList()
            ;
            var qualifiedCategoryIDs = this.DataComponent.GetFullSearchCategories(this.CurrentViewModel.ClientID, categoryIDs);
            var categories = this.DataComponent.GetCategoriesFromList(qualifiedCategoryIDs);
            var models = categories.RowsToModels<ServiceRow>();
            return Json(models.ToDataSourceResult(request));
        }

        public ActionResult _ServiceDetailEditor(int CategoryID)
        {
            var row = this.DataComponent.GetPersonCategory(CategoryID);
            var model = row.RowsToModels<ServiceDetailViewModel>().Single();
            return PartialView(model);
        }

        public ActionResult AttemptClientServiceLink(int CategoryID)
        {
            this.DataComponent.SetClientServiceLink(this.CurrentViewModel.ClientID, CategoryID, true);
            var rows = this.DataComponent.GetClientCategoryConsentVerbiageSingle(this.CurrentViewModel.ClientID, CategoryID);
            var agreements = rows.RowsToModels<AgreementItem>();

            var model = new ServiceLinkProcessData
            {
                Status = agreements.Count > 0 ? ServiceLinkStatus.ConsentRequired : ServiceLinkStatus.LinkSucceeded,
                Success = agreements.Count == 0,
                CategoryID = CategoryID,
                PendingAgreements = agreements
            };

            return PartialView("_ConsentAgreementEditor", model);
        }
    }
}
