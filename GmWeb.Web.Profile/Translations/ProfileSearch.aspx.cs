using System.Web.SessionState;
using System.Configuration;
using System.Web.Caching;
using System.Collections.Specialized;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Profile;
using System.Web;
using System.Linq;
using System.Web.Security;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Text.RegularExpressions;
using System;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Drawing;
using System.Xml;


namespace GmWeb.Web.ProfileApp
{
    partial class ProfileSearch : System.Web.UI.Page
    {
        // Empty ASP controls to fix immediate build issues
        Label 
            LabelError, LabelErrorServices, LabelCurrentClientName, LabelSearchBy,
            LabelCategory, LabelProfileDescription,
            LabelKeywordList, LabelSelectedServiceTitle,

            LabelSelectedActivityEditItem, LabelDescription, LabelStartDate, LabelEndDate,
            LabelBriefDescription, LabelVenue, LabelVenueAddressNo, LabelVenueAddressStreet, 
            LabelVenueCity, LabelVenueSuite, LabelFeeToClient, LabelMaxAttendees,
            LabelCurrentAttendeesEdit, LabelRemainingSlotsEdit, LabelServiceType,
            LabelNeighborhood, LabelVenueAddressStreetType
        ;

        DropDownList DropDownListDataType, DropDownListSelectKeyword, DropDownListThreshold;

        Panel PanelSearchKeywords, PanelSearchProfiles, PanelServices, PanelMatchServices, PanelConsent;

        RadioButton RadioButtonSearchByProfiles, RadioButtonSearchByKeywords;

        Button ButtonPrevious, ButtonNext;

        DataGrid DataGridProfiles, DataGridSelectedServices, DataGridServices, DataGridConsent;
        //********************************************//

        private DataComponent _dataComponent;
        private int _clientID = 0;
        private int _clientDataTypeID = 0;
        private Dictionary<string, bool> _profileList;

        private enum ePanel
        {
            Top,
            SearchProfiles,
            SearchKeywords,
            MatchServices,
            Services,
            Consent
        }

        private enum eProfileColumn
        {
            Select,
            ProfileName,
            Value
        }

        private enum eSelectedProfileColumn
        {
            Select,
            Detail,
            ProfileName
        }



        /// <summary>
    /// Handles the Page Load event. It instantiates a DataComponent object, then sets controls on the page.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Transfer to the logon screen if they're not logged in.
            if (Session.Count == 0 || Session["UserEmailAddress"] == null)
                Server.Transfer("Logon.aspx");

            _dataComponent = new DataComponent();
            _clientID = int.Parse(Session["ClientID"].ToString());
            _clientDataTypeID = ViewState["ClientDataTypeID"] != null ? System.Convert.ToInt32(ViewState["ClientDataTypeID"]) : 0;

            LabelError.Visible = false;
            LabelErrorServices.Visible = false;

            if (Session["ProfileList"] != null)
                _profileList = (Dictionary<string, bool>)Session["ProfileList"];

            if (!IsPostBack)
            {
                BindDDLs();
                ButtonPrevious.Visible = false;
                ButtonNext.Visible = false;
                ShowPanel(ePanel.SearchProfiles);
            }

            // Initialize the client controls.
            if (PanelSearchProfiles.Visible || PanelSearchKeywords.Visible)
                SetupClient();
        }

        /// <summary>
    /// Bind the DDLs.
    /// </summary>
        private void BindDDLs()
        {
            {
                var withBlock = DropDownListDataType;
                withBlock.DataSource = _dataComponent.GetClientDataTypes();
                withBlock.DataBind();
                withBlock.SelectedIndex = 0;
            }
        }

        /// <summary>
    /// Initialize the client controls.
    /// </summary>
        private void SetupClient()
        {
            LabelCurrentClientName.Text = string.Format("{0}, {1}", Session["LastName"], Session["FirstName"]);

            ResetProfileList();

            LabelCategory.Text = string.Empty;
            LabelProfileDescription.Text = string.Empty;

            ButtonPrevious.Visible = false;
            ButtonNext.Visible = false;
        }

        /// <summary>
    /// Destroys and recreates _profileList.
    /// </summary>
        private void ResetProfileList()
        {
            _profileList = new Dictionary<string, bool>();

            string name = string.Empty;
            DataTable table = _dataComponent.GetTemplateProfiles();

            foreach (DataRow row in table.Rows)
            {
                name = row.ToString("Name");
                _profileList.Add(name, false);
            }

            Session["ProfileList"] = _profileList;
        }

        /// <summary>
    /// Copies DataGridProfiles selections to _profileName.
    /// </summary>
        private void CopyGridToProfileList()
        {
            DataGridItem item = null;
            string profileName = string.Empty;
            bool selected = false;

            {
                var withBlock = DataGridProfiles;
                var loopTo = withBlock.Items.Count - 1;
                for (int index = 0; index <= loopTo; index++)
                {
                    item = withBlock.Items[index];

                    profileName = ((Label)item.Cells[(int)eProfileColumn.ProfileName].FindControl("LabelProfileName")).Text;
                    selected = ((CheckBox)item.Cells[(int)eProfileColumn.Select].FindControl("CheckBoxSelectProfile")).Checked;

                    _profileList[profileName] = selected;
                }
            }

            Session["ProfileList"] = _profileList;
        }

        /// <summary>
    /// Copies _profileName selections to DataGridProfiles.
    /// </summary>
        private void CopyProfileListToScreen()
        {
            DataGridItem item = null;
            string profileName = string.Empty;
            bool selected = false;

            {
                var withBlock = DataGridProfiles;
                var loopTo = withBlock.Items.Count - 1;
                for (int index = 0; index <= loopTo; index++)
                {
                    item = withBlock.Items[index];

                    profileName = ((Label)item.Cells[(int)eProfileColumn.ProfileName].FindControl("LabelProfileName")).Text;
                    ((CheckBox)item.Cells[(int)eProfileColumn.Select].FindControl("CheckBoxSelectProfile")).Checked = _profileList[profileName];
                }
            }
        }

        /// <summary>
    /// Handles the RadioButtonSearchByProfiles CheckedChanged event. It sets up the profile search screen.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void RadioButtonSearchByProfiles_CheckedChanged(object sender, System.EventArgs e)
        {
            ShowPanel(ePanel.SearchProfiles);
        }

        /// <summary>
    /// Handles the RadioButtonSearchByProfiles CheckedChanged event. It sets up the keyword search screen.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void RadioButtonSearchByKeywords_CheckedChanged(object sender, System.EventArgs e)
        {
            ShowPanel(ePanel.SearchKeywords);
        }

        /// <summary>
    /// Loads and shows the requested panel. Also sets the client textbox writeability and shows the search by controls.
    /// </summary>
    /// <param name="p">The panel to be shown.</param>
        private void ShowPanel(ePanel p)
        {
            PanelSearchProfiles.Visible = false;
            PanelSearchKeywords.Visible = false;
            PanelMatchServices.Visible = false;
            PanelServices.Visible = false;
            PanelConsent.Visible = false;

            switch (p)
            {
                case ePanel.SearchProfiles:
                    {
                        PanelSearchProfiles.Visible = true;

                        LabelSearchBy.Visible = true;
                        RadioButtonSearchByProfiles.Visible = true;
                        RadioButtonSearchByKeywords.Visible = true;
                        break;
                    }

                case ePanel.SearchKeywords:
                    {
                        LoadSearchKeywords();
                        PanelSearchKeywords.Visible = true;

                        LabelSearchBy.Visible = true;
                        RadioButtonSearchByProfiles.Visible = true;
                        RadioButtonSearchByKeywords.Visible = true;
                        break;
                    }

                case ePanel.MatchServices:
                    {
                        PanelMatchServices.Visible = true;

                        LabelSearchBy.Visible = false;
                        RadioButtonSearchByProfiles.Visible = false;
                        RadioButtonSearchByKeywords.Visible = false;
                        break;
                    }

                case ePanel.Services:
                    {
                        PanelServices.Visible = true;

                        LabelSearchBy.Visible = false;
                        RadioButtonSearchByProfiles.Visible = false;
                        RadioButtonSearchByKeywords.Visible = false;
                        break;
                    }

                case ePanel.Consent:
                    {
                        PanelConsent.Visible = true;
                        break;
                    }
            }
        }

        /// <summary>
    /// Handles the DropDownListDataType SelectedIndexChanged event.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DropDownListDataType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (DropDownListDataType.SelectedIndex <= 0)
                return;

            CopyGridToProfileList();

            _clientDataTypeID = int.Parse(DropDownListDataType.SelectedItem.Value);
            ViewState["ClientDataTypeID"] = _clientDataTypeID;

            LoadSearchProfiles();
        }

        /// <summary>
    /// Handles the ButtonPrevious Click event. It validates and saves the current data, then transfers to the previous screen.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonPrevious_Click(object sender, System.EventArgs e)
        {
            DataTable table = _dataComponent.GetClientDataType(_clientDataTypeID);
            DataRow row = table.Rows[0];
            int prevClientDataTypeID = row.ToInteger("PrevClientDataTypeID");

            if (prevClientDataTypeID > 0)
            {
                CopyGridToProfileList();
                _clientDataTypeID = prevClientDataTypeID;
                ViewState["ClientDataTypeID"] = _clientDataTypeID;
                LoadSearchProfiles();
            }
        }

        /// <summary>
    /// Handles the ButtonNext Click event. It validates and saves the current data, then transfers to the next screen.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonNext_Click(object sender, System.EventArgs e)
        {
            DataTable table = _dataComponent.GetClientDataType(_clientDataTypeID);
            DataRow row = table.Rows[0];
            int nextClientDataTypeID = row.ToInteger("NextClientDataTypeID");

            if (nextClientDataTypeID > 0)
            {
                CopyGridToProfileList();
                _clientDataTypeID = nextClientDataTypeID;
                ViewState["ClientDataTypeID"] = _clientDataTypeID;
                LoadSearchProfiles();
            }
        }

        /// <summary>
    /// Loads the profile search screen.
    /// </summary>
        private void LoadSearchProfiles()
        {
            DataGridItem item = default(DataGridItem);
            // Declare local data.
            Label labelFundingValue = default(Label);
            DataRow rowProfile = default(DataRow);
            string name = string.Empty;

            DataTable tableProfile = _dataComponent.GetProfileSetupData("CLI", _clientID, _clientDataTypeID);
            DataTable tableDDL = default(DataTable);

            DataTable tableClient = _dataComponent.GetClientData(_clientID);
            DataRow rowClient = tableClient.Rows[0];

            DataTable tableAssociation = _dataComponent.GetAssociationData(rowClient.ToInteger("PrimaryAssociationID"));
            DataRow rowAssociation = (tableAssociation.Rows.Count > 0) ? tableAssociation.Rows[0] : null;

            // Load DataGridProfiles.
            {
                var withBlock = DataGridProfiles;
                withBlock.DataSource = tableProfile;
                withBlock.DataBind();
                var loopTo = withBlock.Items.Count - 1;

                // Run through each row of the DataGrid, showing and loading the controls by data type.
                for (int index = 0; index <= loopTo; index++)
                {
                    item = withBlock.Items[index];
                    labelFundingValue = (Label)item.Cells[(int)eProfileColumn.Value].FindControl("LabelFundingValue");

                    rowProfile = tableProfile.Rows[index];
                    name = rowProfile.ToString("Name");

                    switch (rowProfile.ToString("DataType"))
                    {
                        case "S":
                            {
                                labelFundingValue.Text = rowProfile.ToString("Value");
                                break;
                            }

                        case "N":
                            {
                                labelFundingValue.Text = rowProfile.ToString("Value");
                                break;
                            }

                        case "D":
                            {
                                labelFundingValue.Text = rowProfile.ToString("Value");
                                break;
                            }

                        case "B":
                            {
                                labelFundingValue.Text = (rowProfile.ToBoolean("Value") == true) ? "Yes" : string.Empty;
                                break;
                            }

                        case "L":
                            {
                                labelFundingValue.Text = string.Empty;
                                int lookupValue = rowProfile.ToInteger("Value");

                                if (lookupValue != 0)
                                {
                                    tableDDL = _dataComponent.GetProfileDDLData(rowProfile.ToString("LookupTableName"));

                                    foreach (DataRow row in tableDDL.Rows)
                                    {
                                        if (lookupValue == row.ToInteger("ID"))
                                        {
                                            labelFundingValue.Text = row.ToString("Description");
                                            break;
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }
            }

            LoadDialogControls();
            CopyProfileListToScreen();
        }

        /// <summary>
    /// Loads and enables the category and description; enables the previous and next buttons appropriately.
    /// </summary>
        private void LoadDialogControls()
        {
            DataTable table = _dataComponent.GetClientDataType(_clientDataTypeID);
            DataRow row = table.Rows[0];

            LabelCategory.Text = row.ToString("Category");
            LabelProfileDescription.Text = row.ToString("Description");

            ButtonPrevious.Enabled = (row.ToInteger("PrevClientDataTypeID") > 0) ? true : false;
            ButtonNext.Enabled = (row.ToInteger("NextClientDataTypeID") > 0) ? true : false;

            ButtonPrevious.Visible = (DataGridProfiles.Items.Count > 0);
            ButtonNext.Visible = ButtonPrevious.Visible;
        }

        /// <summary>
    /// Loads the keyword search screen.
    /// </summary>
        private void LoadSearchKeywords()
        {
            {
                var withBlock = DropDownListSelectKeyword;
                if (withBlock.Items.Count == 0)
                {
                    withBlock.DataSource = _dataComponent.GetServiceKeywords();
                    withBlock.DataBind();
                    if (withBlock.Items.Count > 0)
                        withBlock.SelectedIndex = 0;
                }
            }

            LabelKeywordList.Text = string.Empty;
        }

        /// <summary>
    /// Loads the match services screen.
    /// </summary>
        private void LoadMatchServices(string categoryIDList, string searchType)
        {
            LabelSelectedServiceTitle.Text = string.Format("These services match the {0}(s) you've selected.", searchType);

            {
                var withBlock = DataGridSelectedServices;
                withBlock.DataSource = _dataComponent.GetCategoriesFromList(categoryIDList);
                withBlock.DataBind();
            }

            ButtonResetServiceSearch_Click(this, new EventArgs());
        }

        /// <summary>
    /// Handles the ButtonFullSearch Click event. It creates a comma-separated list of selected categories, checks if they
    /// fit the client profile, then loads the services panel.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonFullSearch_Click(object sender, System.EventArgs e)
        {
            int categoryID = 0;
            DataGridItem item = null;
            int count = 0;
            StringBuilder sb = new StringBuilder();
            string clientServiceProfileID = string.Empty;

            {
                var withBlock = DataGridSelectedServices;
                var loopTo = withBlock.Items.Count - 1;
                for (int index = 0; index <= loopTo; index++)
                {
                    item = withBlock.Items[index];
                    if (((CheckBox)item.Cells[(int)eSelectedProfileColumn.Select].FindControl("CheckBoxSelectService")).Checked)
                    {
                        categoryID = System.Convert.ToInt32(withBlock.DataKeys[index]);
                        sb.AddMessage(categoryID.ToString(), ",");
                        count += 1;
                    }
                }
            }

            string categoryIDList = _dataComponent.GetFullSearchCategories(_clientID, sb.ToString().Split(',').Select(x => int.Parse(x)).ToList());

            if (count > 0)
            {
                LoadServices(categoryIDList);
                ShowPanel(ePanel.Services);
            }
            else
                ShowMessage(LabelError, "No profiles were checked.");
        }

        /// <summary>
    /// Handles the DataGridSelectedServices ItemCommand event. If the Detail link is clicked, it displays a dialog with
    /// the details of the service.
    /// </summary>
    /// <param name="source">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DataGridSelectedServices_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            int categoryID = System.Convert.ToInt32(DataGridSelectedServices.DataKeys[e.Item.ItemIndex]);

            switch (e.CommandName)
            {
                case "ShowDetail":
                    {
                        LoadServiceDetailDialog(categoryID);
                        OpenServiceDetailDialog();
                        break;
                    }
            }
        }

        /// <summary>
    /// Handles the ButtonResetServiceSearch Click event. It clears the checkboxes in DataGridSelectedServices.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonResetServiceSearch_Click(object sender, System.EventArgs e)
        {
            DataGridItem item = null;
            var loopTo = DataGridSelectedServices.Items.Count - 1;
            for (int index = 0; index <= loopTo; index++)
            {
                item = DataGridSelectedServices.Items[index];
                ((CheckBox)item.Cells[(int)eSelectedProfileColumn.Select].FindControl("CheckBoxSelectService")).Checked = false;
            }
        }

        /// <summary>
    /// Loads DataGridServices.
    /// </summary>
    /// <param name="categoryIDList"></param>
        private void LoadServices(string categoryIDList)
        {
            {
                var withBlock = DataGridServices;
                withBlock.DataSource = _dataComponent.GetCategoriesFromList(categoryIDList);
                withBlock.DataBind();
            }
        }

        /// <summary>
    /// Handles the ButtonSearchProfiles Click event. It first ensures that a client has been selected. Then it assures
    /// that each of the selected services have been entered by the client. It then collects a csv list of
    /// profile names, finds categories that match the client for these names and loads the match services screen.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonSearchProfiles_Click(object sender, System.EventArgs e)
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();

            CopyGridToProfileList();

            foreach (string name in _profileList.Keys)
            {
                if (_profileList[name])
                {
                    if (_dataComponent.GetClientProfile(_clientID, name).Rows.Count == 0)
                        sb.AddMessage(string.Format("The {0} profile does not exist for this client.", name));
                }
            }

            if (sb.Length > 0)
            {
                ShowMessage(LabelError, sb.ToString());
                return;
            }

            sb.Length = 0;

            foreach (string name in _profileList.Keys)
            {
                if (_profileList[name])
                {
                    sb.AddMessage(name, ",");
                    count += 1;
                }
            }

            double threshold = (double)Convert.ToSingle(DropDownListThreshold.SelectedItem.Value);
            string categoryIDList = _dataComponent.GetClientCategoryCompareList(_clientID, sb.ToString(), threshold);

            if (count > 0)
            {
                LoadMatchServices(categoryIDList, "profile");
                ShowPanel(ePanel.MatchServices);
            }
            else
                ShowMessage(LabelError, "No profiles were checked.");
        }

        /// <summary>
    /// Handles the ButtonResetProfiles Click event. It resets the checkboxes in DataGridProfiles.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonResetProfiles_Click(object sender, System.EventArgs e)
        {
            DataGridItem item = null;
            var loopTo = DataGridProfiles.Items.Count - 1;
            for (int index = 0; index <= loopTo; index++)
            {
                item = DataGridProfiles.Items[index];
                ((CheckBox)item.Cells[(int)eProfileColumn.Select].FindControl("CheckBoxSelectProfile")).Checked = false;
            }

            ResetProfileList();
        }

        /// <summary>
    /// Handles the DropDownListSelectKeyword SelectedIndexChanged event. It adds the selected keywords from DropDownListSelectKeyword
    /// into LabelKeywordList
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DropDownListSelectKeyword_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (LabelKeywordList.Text == string.Empty)
                LabelKeywordList.Text = DropDownListSelectKeyword.SelectedItem.Value;
            else
                LabelKeywordList.Text += ("," + DropDownListSelectKeyword.SelectedItem.Value);

            DropDownListSelectKeyword.SelectedIndex = 0;
        }

        /// <summary>
    /// Handles the ButtonSearchKeywords Click event. It first ensures that a client has been selected.
    /// Then it gets a categoryID list from services that match the client and have those keywords. It
    /// then shows the match services panel.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonSearchKeywords_Click(object sender, System.EventArgs e)
        {
            if (LabelKeywordList.Text != string.Empty)
            {
                string categoryIDList = _dataComponent.GetKeywordCategoryList(LabelKeywordList.Text.Split(',').ToList());
                LoadMatchServices(categoryIDList, "keyword");
                ShowPanel(ePanel.MatchServices);
            }
            else
                ShowMessage(LabelError, "No keywords have been entered.");
        }

        /// <summary>
    /// Handles the ButtonResetKeywords Click event. It resets the search keyword controls.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonResetKeywords_Click(object sender, System.EventArgs e)
        {
            DropDownListSelectKeyword.SelectedIndex = 0;
            LabelKeywordList.Text = string.Empty;
        }

        /// <summary>
    /// Handles the ButtonResetLastKeyword Click event. It deletes the last keyword from the keyword list.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonResetLastKeyword_Click(object sender, System.EventArgs e)
        {
            int locLast = LabelKeywordList.Text.LastIndexOf(',');
            if (locLast >= 0)
                LabelKeywordList.Text = LabelKeywordList.Text.Substring(0, locLast);
            else
                LabelKeywordList.Text = string.Empty;
        }

        /// <summary>
    /// Handles the DataGridServices ItemCommand event. If the Detail link is clicked, it displays a dialog with
    /// the details of the service. If the Connect link is clicked, creates a tblClientCategory row for this client and category.
    /// </summary>
    /// <param name="source">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DataGridServices_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            int categoryID = System.Convert.ToInt32(DataGridServices.DataKeys[e.Item.ItemIndex]);

            switch (e.CommandName)
            {
                case "ShowDetail":
                    {
                        LoadServiceDetailDialog(categoryID);
                        OpenServiceDetailDialog();
                        DataGridServices.SelectedIndex = e.Item.ItemIndex;
                        break;
                    }

                case "LinkClientToService":
                    {
                        _dataComponent.SetClientServiceLink(_clientID, categoryID, true);

                        DataTable table = _dataComponent.GetClientCategoryConsentVerbiageSingle(_clientID, categoryID);
                        if (table.Rows.Count > 0)
                        {
                            LoadClientConsentAgreements(table);
                            ShowPanel(ePanel.Consent);
                        }
                        else
                            ShowMessage(LabelErrorServices, "This client and service have been linked.", true);

                        DataGridServices.SelectedIndex = e.Item.ItemIndex;
                        break;
                    }
            }
        }

        /// <summary>
    /// Loads the service detail dialog.
    /// </summary>
        private void LoadServiceDetailDialog(int categoryID)
        {
            DataTable table = _dataComponent.GetPersonCategory(categoryID);
            DataRow row = table.Rows[0];

            LabelSelectedActivityEditItem.Text = row.ToString("ActivityDescription");
            LabelDescription.Text = row.ToString("Description");
            LabelStartDate.Text = row.ToDateTime("StartDate").ToString("MMM d, yyyy");
            LabelEndDate.Text = row.ToDateTime("EndDate").ToString("MMM d, yyyy");
            LabelBriefDescription.Text = row.ToString("BriefDescription");

            LabelVenue.Text = row.ToString("Venue");
            LabelVenueAddressNo.Text = row.ToString("VenueAddressNo");
            LabelVenueAddressStreet.Text = row.ToString("VenueAddressStreet");
            LabelVenueCity.Text = string.Format("{0}, {1} {2}", row.ToString("City"), row.ToString("State"), row.ToString("VenueZip"));
            LabelVenueSuite.Text = row.ToString("VenueSuite");

            LabelFeeToClient.Text = row.ToDecimal("FeeToClient").ToString("c");
            LabelMaxAttendees.Text = row.ToString("MaxAttendees");
            string attendees = row.ToString("CurrentAttendees"), slots = row.ToString("RemainingSlots");
            LabelCurrentAttendeesEdit.Text = string.IsNullOrEmpty(attendees) ? "0" : attendees;
            LabelRemainingSlotsEdit.Text = string.IsNullOrEmpty(slots) ? "0" : slots;

            LabelServiceType.Text = row.ToString("ServiceTypeDescription");
            LabelNeighborhood.Text = row.ToString("NeighborhoodDescr");
            LabelVenueAddressStreetType.Text = row.ToString("VenueAddressStreetType");
        }

        /// <summary>
    /// Opens the service detail dialog.
    /// </summary>
        private void OpenServiceDetailDialog()
        {
            // Dim script As String = "<script language='javascript'>$(function() {{$('#serviceDetailDialog').dialog('open');}});</script>"
            // Me.ClientScript.RegisterStartupScript(Me.GetType(), "ServiceDetail", script)
            this.ClientScript.RegisterStartupScript(this.GetType(), "Edit", "<script>$('#serviceDetailDialog').modal('show');</script>");
        }

        /// <summary>
    /// Load each of the client consent agreement panels and grids.
    /// </summary>
    /// <remarks></remarks>
        private void LoadClientConsentAgreements(DataTable table)
        {
            // Show client category consent agreements.
            {
                var withBlock = DataGridConsent;
                withBlock.DataSource = table;
                withBlock.DataBind();
            }
        }

        /// <summary>
    /// Handles the ButtonConsent Click event. It inserts consent rows into tblClientCategoryConsent.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonConsent_Click(object sender, System.EventArgs e)
        {
            ProcessConsentDecline(true);
        }

        /// <summary>
    /// Handles the ButtonDecline Click event. It inserts decline rows into tblClientCategoryConsent.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonDecline_Click(object sender, System.EventArgs e)
        {
            ProcessConsentDecline(false);
        }

        /// <summary>
    /// Inserts tblClientCategoryConsent row. On consent, sets the client service links, loads DataGridConsent and displays a success message.
    /// On decline, simply shows a decline message. When done shows the Services panel.
    /// </summary>
    /// <param name="consent">Agreement consent or decline.</param>
        private void ProcessConsentDecline(bool consent)
        {
            int categoryID = System.Convert.ToInt32(DataGridServices.DataKeys[DataGridServices.SelectedIndex]);

            if (consent)
            {
                DataTable table = _dataComponent.GetClientCategoryConsentVerbiageSingle(_clientID, categoryID);
                LoadClientConsentAgreements(table);

                ShowMessage(LabelErrorServices, "This client and service have been linked.", true);
            }
            else
            {
                _dataComponent.SetClientServiceLink(_clientID, categoryID, false);
                ShowMessage(LabelErrorServices, "This client and service have not been linked because the agency agreements have been declined.", true);
            }

            _dataComponent.InsertClientCategoryConsentSingle(_clientID, categoryID, consent, Session["UserEmailAddress"].ToString());
            ShowPanel(ePanel.Services);
        }



        /// <summary>
    /// Displays the message in red if it's an error or blue if it's a message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="isMessage">True if message; else False.</param>
        private void ShowMessage(Label errorLabel, string message, bool isMessage = false)
        {
            var messageColor = isMessage ? Color.FromArgb(unchecked((int)0xFF0060A0)) : Color.Red;

            errorLabel.Visible = true;
            errorLabel.ForeColor = messageColor;
            errorLabel.Text = message;
        }
    }
}
