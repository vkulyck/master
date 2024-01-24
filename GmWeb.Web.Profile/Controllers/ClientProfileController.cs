using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GmWeb.Web.Profile.Models;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Web.Profile.Models.Shared;
using GmWeb.Web.Profile.Models.ClientProfile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

// TODO: remove these
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GmWeb.Web.Profile.Controllers
{
    public class ClientProfileController : BaseClientServicesController<ClientProfileViewModel>
    {
        public ActionResult Index(int? ClientDataTypeID, bool ProfileOnly = true)
        {
            //ClientDataTypeID = ClientDataTypeID ?? this.CurrentUser.DefaultClientDataTypeID;
            var dc = this.DataComponent;
            var clientDataType = dc.GetClientDataType(ClientDataTypeID.Value).RowsToModels<ClientDataType>().Single();
            var volunteer = dc.GetVolunteer(this.CurrentUser.Email).RowsToModels<Volunteer>().FirstOrDefault();
            var model = this.CreateViewModel(m =>
            {
                m.Description = clientDataType.Description;
                m.Category = clientDataType.Category;
                m.ClientSupplementalData = ProfileOnly ? "Client Profile Data" : "Client Supplemental Data";
                m.Volunteer = volunteer;
                m.ClientGroupID = ClientDataTypeID.Value;
                m.PrevClientDataTypeID = clientDataType.PrevClientDataTypeID;
                m.ClientDataTypeID = ClientDataTypeID.Value;
                m.NextClientDataTypeID = clientDataType.NextClientDataTypeID;
                m.ProfileOnly = ProfileOnly;
            });

            return View(model);
        }

        public ActionResult GetClientDataTypes()
        {
            var results = this.DataComponent.GetClientDataTypes().RowsToModels<ClientDataType>();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _SupplementalGrid()
        {
            return PartialView();
        }
        private enum gridColumnsSupplemental // TODO: remove or refactor
        {
            EligibilityName,
            Op,
            Value
        }
        [HttpPost]
        public ActionResult GetSupplementalGridData([DataSourceRequest]DataSourceRequest request)
        {
            var rows = new List<SupplementalGridRow>();
            var datatable_supplemental = this.DataComponent.GetSupplementalSetupData(this.CurrentViewModel.ClientID, this.CurrentViewModel.ClientDataTypeID, this.CurrentViewModel.ProfileOnly);
            var datatable_client = this.DataComponent.GetClientData(this.CurrentViewModel.ClientID);
            DataRow datarow_client = datatable_client.Rows[0];
            DataTable datatable_association = this.DataComponent.GetAllAssociationData(datarow_client.ToInteger("PrimaryAssociationID"));
            DataRow datarow_association = (datatable_association.Rows.Count > 0) ? datatable_association.Rows[0] : null;

            foreach(DataRow datarow_supplemental in datatable_supplemental.Rows)
            {
                var model = new SupplementalGridRow
                {
                    Name = datarow_supplemental.ToString("Name"),
                    ClientID = datarow_supplemental.ToNullableInteger("ClientID"),
                    DataType = datarow_supplemental.ToString("DataType"),
                    FundingNumericValue = datarow_supplemental.ToString("Value"),
                    FundingBitValue = datarow_supplemental.ToNullableBoolean("Value"),
                    FundingListSelectionValue = datarow_supplemental.ToString("Value"),
                    FundingValue = datarow_supplemental.ToString("Value"),
                };
                if (model.ClientID == null)
                {
                    model.SetFundingNumeric(datarow_client, datarow_association);
                    model.SetFundingBit(datarow_client, datarow_association);
                    model.SetFundingListSelection(datarow_client, datarow_association);
                }
                model.SetOperator();
                if(model.DataType == "L")
                {
                    var lookupTableName = datarow_supplemental.ToString("LookupTableName");
                    var settings = new SupplementalGridRowListColumnSettings
                    {
                        ProfileName = model.Name,
                        LookupTableName = lookupTableName
                    };
                    var table = this.DataComponent.GetProfileDDLData(lookupTableName);
                    settings.SetItems(model.DataType, table);
                    model.ListSettings = settings;
                }
                rows.Add(model);
            }
            return Json(rows.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveSupplementalData(int ClientDataTypeID, IEnumerable<SupplementalGridRow> data)
        {
            foreach(var row in data)
            {
                if (string.IsNullOrEmpty(row.FundingValue))
                    this.DataComponent.DeleteProfileSetupSingle("CLI", this.CurrentViewModel.ClientID, row.Name, ClientDataTypeID);
                else
                    this.DataComponent.AddProfileSetupSingle("CLI", this.CurrentViewModel.ClientID, row.Name, row.FundingValue, row.ListSettings?.LookupTableName, row.DataType, ClientDataTypeID, this.CurrentUser.Email);
            }
            return Json(new { success = true });
        }
    }
}
