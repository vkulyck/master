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
using System.Net.Mail;
using System.Drawing;

namespace GmWeb.Web.ProfileApp
{
    partial class ClientSupplementalData : System.Web.UI.Page
    {
        // Empty ASP controls to fix immediate build issues
        Label LabelClientSupplementalData, LabelErrorSupplemental, LabelCategory, LabelDescription;
        Label
            LabelBirthday, LabelGender, LabelEthnicity, LabelCulturalIdentityID,
            LabelLanguageIDPrimary, LabelLanguageIDSecondary, LabelEmail,
            LabelPhone, LabelAddress, LabelAddressLine2
        ;

        Button ButtonPrevious, ButtonNext;
        DataGrid DataGridSupplemental;
        //********************************************//

        private DataComponent _dataComponent;
        // Private _clientID As Integer
        // Private _clientDataTypeID As Integer
        public int _clientID;
        public int _clientDataTypeID;
        public int _clientGroupID;
        private bool _profileOnly;

        public int ClntID;
        public string ClientFullName;
        public int AgencyIDParent;

        private enum gridColumnsSupplemental
        {
            EligibilityName,
            Op,
            Value
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

            // Retrieve the QueryStrings.
            _clientID = int.Parse(Request.QueryString["ClientID"]);
            _profileOnly = (Request.QueryString["ProfileOnly"] == "Y");

            if (ViewState["ClientDataTypeID"] == null)
                _clientDataTypeID = int.Parse(Request.QueryString["ClientDataTypeID"]);
            else
                _clientDataTypeID = int.Parse(ViewState["ClientDataTypeID"].ToString());

            _clientGroupID = int.Parse(Request.QueryString["ClientDataTypeID"]);

            LabelClientSupplementalData.Text = _profileOnly ? "Client Profile Data" : "Client Supplemental Data";

            // Hide the error and message labels.
            LabelErrorSupplemental.Visible = false;

            if (!IsPostBack)
                LoadControls();

            ClientFullName = System.Convert.ToString(Session["FirstName"]) + " " + System.Convert.ToString(Session["LastName"]);
            ClntID = System.Convert.ToInt32(Session["ClientID"]);
            AgencyIDParent = System.Convert.ToInt32(Session["AgencyIDParent"]);
        }

        /// <summary>
    /// Loads this screen's controls.
    /// </summary>
        private void LoadControls()
        {
            DataTable table = _dataComponent.GetClientDataType(_clientDataTypeID);
            DataRow row = table.Rows[0];

            LabelCategory.Text = row.ToString("Category");
            LabelDescription.Text = row.ToString("Description");

            ButtonPrevious.Enabled = (row.ToInteger("PrevClientDataTypeID") > 0) ? true : false;
            ButtonNext.Enabled = (row.ToInteger("NextClientDataTypeID") > 0) ? true : false;

            LoadSupplementalGrid();
            GetVolunteer();
        }

        /// <summary>
    /// Loads the supplemental data grid.
    /// </summary>
        private void LoadSupplementalGrid()
        {
            DataGridItem item = default(DataGridItem);
            TextBox tbFundingValue = default(TextBox);
            CheckBox cbFundingValue = default(CheckBox);
            // Declare local data.
            DropDownList ddlFundingValue = default(DropDownList);
            DataRow rowSupplemental = default(DataRow);
            ListItem li = default(ListItem);
            string name = string.Empty;

            DataTable tableSupplemental = _dataComponent.GetSupplementalSetupData(_clientID, _clientDataTypeID, _profileOnly);
            DataTable tableDDL = default(DataTable);

            DataTable tableClient = _dataComponent.GetClientData(_clientID);
            DataRow rowClient = tableClient.Rows[0];

            DataTable tableAssociation = _dataComponent.GetAllAssociationData(rowClient.ToInteger("PrimaryAssociationID"));
            DataRow rowAssociation = (tableAssociation.Rows.Count > 0) ? tableAssociation.Rows[0] : null;

            // Load DataGridSupplemental.
            {
                var withBlock = DataGridSupplemental;
                withBlock.DataSource = tableSupplemental;
                withBlock.DataBind();

                if (tableSupplemental.Rows.Count == 0)
                    ShowMessage(LabelErrorSupplemental, "There is no data for this group.", true);
                var loopTo = withBlock.Items.Count - 1;

                // Run through each row of the DataGrid, showing and loading the controls by data type.
                for (int index = 0; index <= loopTo; index++)
                {
                    item = withBlock.Items[index];

                    tbFundingValue = (TextBox)item.Cells[(int)gridColumnsSupplemental.Value].FindControl("TextBoxFundingValue");
                    cbFundingValue = (CheckBox)item.Cells[(int)gridColumnsSupplemental.Value].FindControl("CheckBoxFundingValue");
                    ddlFundingValue = (DropDownList)item.Cells[(int)gridColumnsSupplemental.Value].FindControl("DropDownListFundingValue");

                    tbFundingValue.Visible = false;
                    cbFundingValue.Visible = false;
                    ddlFundingValue.Visible = false;

                    rowSupplemental = tableSupplemental.Rows[index];
                    name = rowSupplemental.ToString("Name");

                    switch (rowSupplemental.ToString("DataType"))
                    {
                        case "S":
                            {
                                tbFundingValue.Visible = true;
                                ((Label)item.Cells[(int)gridColumnsSupplemental.Op].FindControl("LabelOp")).Text = "=";

                                if (rowSupplemental["ClientID"] == DBNull.Value)
                                    tbFundingValue.Text = GetStringOrNumericDefault(name, rowClient, rowAssociation);
                                else
                                    tbFundingValue.Text = rowSupplemental.ToString("Value");
                                break;
                            }
                            break;

                        case "N":
                            {
                                tbFundingValue.Visible = true;
                                ((Label)item.Cells[(int)gridColumnsSupplemental.Op].FindControl("LabelOp")).Text = "<=";

                                if (rowSupplemental["ClientID"] == DBNull.Value)
                                    tbFundingValue.Text = GetStringOrNumericDefault(name, rowClient, rowAssociation);
                                else
                                    tbFundingValue.Text = rowSupplemental.ToString("Value");
                                break;
                            }
                            break;

                        case "D":
                            {
                                tbFundingValue.Visible = true;
                                ((Label)item.Cells[(int)gridColumnsSupplemental.Op].FindControl("LabelOp")).Text = "=";

                                if (rowSupplemental["ClientID"] == DBNull.Value)
                                    tbFundingValue.Text = GetStringOrNumericDefault(name, rowClient, rowAssociation);
                                else
                                    tbFundingValue.Text = rowSupplemental.ToString("Value");
                                break;
                            }
                            break;

                        case "B":
                            {
                                cbFundingValue.Visible = true;
                                if (rowSupplemental["ClientID"] == DBNull.Value)
                                    cbFundingValue.Checked = GetBooleanDefault(name, rowClient, rowAssociation);
                                else
                                    cbFundingValue.Checked = rowSupplemental.ToBoolean("Value");
                                break;
                            }
                            break;

                        case "L":
                            {
                                {
                                    var withBlock1 = ddlFundingValue;
                                    withBlock1.Visible = true;

                                    tableDDL = _dataComponent.GetProfileDDLData(rowSupplemental.ToString("LookupTableName"));

                                    switch (rowSupplemental.ToString("LookupTableName"))
                                    {
                                        case "lkpEducationLevel":
                                            {
                                                tableDDL.DefaultView.Sort = "ID";
                                                break;
                                            }

                                        default:
                                            {
                                                tableDDL.DefaultView.Sort = "Description";
                                                break;
                                            }
                                    }

                                    withBlock1.DataSource = tableDDL;
                                    withBlock1.DataBind();

                                    switch (name)
                                    {
                                        case "Head of Household":
                                            {
                                                li = new ListItem(" (None)", "0");
                                                break;
                                            }

                                        case "Occupation Type":
                                            {
                                                li = new ListItem("Not Applicable", "0");
                                                break;
                                            }

                                        default:
                                            {
                                                li = new ListItem(" (Select)", "0");
                                                break;
                                            }
                                    }

                                    ddlFundingValue.Items.Insert(0, li);

                                    if (rowSupplemental["ClientID"] == DBNull.Value)
                                        withBlock1.SetValue(GetLookupDefault(name, rowClient, rowAssociation));
                                    else
                                        withBlock1.SetValue(rowSupplemental, "Value");
                                }

                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
    /// It loads the client's information.
    /// </summary>
    /// <remarks></remarks>
        public void GetVolunteer()
        {
            // Fetch the user's information.
            DataTable tableUser = _dataComponent.GetVolunteer(System.Convert.ToString(Session["UserEmailAddress"]));

            // Check if any user matched the value entered.
            if (tableUser.Rows.Count > 0)
            {
                // Extract the user's data into a DataRow.
                DataRow rowUser = tableUser.Rows[0];

                LabelBirthday.Text = rowUser.ToString("Birthday");
                LabelGender.Text = rowUser.ToString("Gender").Replace("M", "Male").Replace("F", "Female").Replace("T", "Transgender").Replace("U", "Unidentified");
                LabelEthnicity.Text = rowUser.ToString("EthDescription");
                LabelCulturalIdentityID.Text = rowUser.ToString("CulturalIdentityDescription");
                LabelLanguageIDPrimary.Text = rowUser.ToString("LanguagePrimaryDescription");
                LabelLanguageIDSecondary.Text = rowUser.ToString("LanguageSecondaryDescription");
                LabelEmail.Text = rowUser.ToString("Email");
                LabelPhone.Text = rowUser.ToString("Phone");
                LabelAddress.Text = rowUser.ToString("AddressNo") + " " + rowUser.ToString("AddressStreet") + " " + rowUser.ToString("AddressStreetType") + ", " + rowUser.ToString("City") + ", " + rowUser.ToString("Zip");
                LabelAddressLine2.Text = rowUser.ToString("AddressLine2");
            }
        }

        /// <summary>
    /// Returns a default string or numeric value for each of the profile names if there is one; if not, it returns an empty string.
    /// </summary>
    /// <param name="name">The profile name.</param>
    /// <param name="rowClient">This client's client data.</param>
    /// <param name="rowAssociation">This client's association data.</param>
    /// <returns>Default value if it exists; else an empty string.</returns>
        private string GetStringOrNumericDefault(string name, DataRow rowClient, DataRow rowAssociation)
        {
            string value = string.Empty;

            switch (name)
            {
                case "Household Income level":
                    {
                        value = rowClient.ToString("TotalHouseholdIncome");
                        break;
                    }

                case "Size of Household":
                    {
                        value = rowClient.ToString("NumberInHousehold");
                        break;
                    }

                case "Family Income Level":
                    {
                        value = rowClient.ToString("TotalFamilyIncome");
                        break;
                    }

                case "Family Size":
                    {
                        value = rowClient.ToString("NumberInFamily");
                        break;
                    }

                case "Residence":
                    {
                        value = rowClient.ToString("Residence");
                        break;
                    }

                case "Age":
                    {
                        value = rowClient.ToString("ClientAge");
                        break;
                    }

                default:
                    {
                        try
                        {
                            var col = name.Replace(" ", "");
                            value = rowAssociation != null ? rowAssociation.ToString(col) : string.Empty;
                        }
                        catch
                        {
                            value = string.Empty;
                        }

                        break;
                    }
            }

            // When a numeric value ends with .0000, change it to .00.
            if (value.EndsWith(".0000"))
                value = value.Substring(0, value.Length - 2);

            return value;
        }

        /// <summary>
    /// Returns a default boolean value for each of the profile names if there is one; if not, it returns False.
    /// </summary>
    /// <param name="name">The profile name.</param>
    /// <param name="rowClient">This client's client data.</param>
    /// <param name="rowAssociation">This client's association data.</param>
    /// <returns>Default value if it exists; else False.</returns>
        private bool GetBooleanDefault(string name, DataRow rowClient, DataRow rowAssociation)
        {
            bool value = false;

            switch (name)
            {
                case "HIV Status":
                    {
                        value = rowClient.ToBoolean("AIDS");
                        break;
                    }

                case "Currently Employed":
                    {
                        value = rowClient.ToBoolean("CurrentEmployment");
                        break;
                    }

                case "Disabled":
                    {
                        value = rowClient.ToBoolean("SevereDisabilities");
                        break;
                    }

                default:
                    {
                        try
                        {
                            value = rowAssociation != null ? rowAssociation.ToBoolean(name.Replace(" ", "")) : false;
                        }
                        catch
                        {
                            value = false;
                        }

                        break;
                    }
            }

            return value;
        }

        /// <summary>
    /// Returns a default lookup value for each of the profile names if there is one; if not, it returns "0".
    /// </summary>
    /// <param name="name">The profile name.</param>
    /// <param name="rowClient">This client's client data.</param>
    /// <param name="rowAssociation">This client's association data.</param>
    /// <returns>Default value if it exists; else "0".</returns>
        private string GetLookupDefault(string name, DataRow rowClient, DataRow rowAssociation)
        {
            string value = "0";

            switch (name)
            {
                case "Head of Household":
                    {
                        value = rowClient.ToString("HeadofHouseholdType");
                        break;
                    }

                case "Employment Type":
                    {
                        value = rowClient.ToString("EmploymentTypeID");
                        break;
                    }

                case "Education level":
                    {
                        value = rowClient.ToString("TargetPopulationEducationLevelID");
                        break;
                    }

                case "Language Proficiency":
                    {
                        value = rowClient.ToString("LanguageProficiencyID");
                        break;
                    }

                case "Language Primary":
                    {
                        value = rowAssociation != null ? rowAssociation.ToString("LanguageIDPrimary") : string.Empty;
                        break;
                    }

                case "Language Secondary":
                    {
                        value = rowAssociation != null ? rowAssociation.ToString("LanguageIDSecondary") : string.Empty;
                        break;
                    }

                case "EEO Industry Code":
                    {
                        value = rowAssociation != null ? rowAssociation.ToString("EEOIndustryCode") : string.Empty;
                        break;
                    }

                case "Status":
                    {
                        value = rowAssociation != null ? rowAssociation.ToString("Status") : string.Empty;
                        break;
                    }

                default:
                    {
                        try
                        {
                            var col = name.Replace(" ", "") + "ID";
                            value = rowAssociation != null ? rowAssociation.ToString(col) : string.Empty;
                        }
                        catch
                        {
                            value = string.Empty;
                        }

                        break;
                    }
            }

            if (value == string.Empty)
                return "0";
            return value;
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
                table = _dataComponent.GetSupplementalSetupData(_clientID, _clientDataTypeID, _profileOnly);
                if (IsValidSupplementalData(table))
                {
                    SaveSupplementalData(table);

                    _clientDataTypeID = prevClientDataTypeID;
                    ViewState["ClientDataTypeID"] = _clientDataTypeID;
                    LabelErrorSupplemental.Visible = false;
                    LoadControls();
                }
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
                table = _dataComponent.GetSupplementalSetupData(_clientID, _clientDataTypeID, _profileOnly);
                if (IsValidSupplementalData(table))
                {
                    SaveSupplementalData(table);

                    _clientDataTypeID = nextClientDataTypeID;
                    ViewState["ClientDataTypeID"] = _clientDataTypeID;
                    LabelErrorSupplemental.Visible = false;
                    LoadControls();
                }
            }
        }

        /// <summary>
    /// Handles the ButtonSupplementalSave Click event. If validates the grid data, deletes current data for this client, then re-adds it.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonSupplementalSave_Click(object sender, System.EventArgs e)
        {
            // Get existing client setup data.
            DataTable table = _dataComponent.GetSupplementalSetupData(_clientID, _clientDataTypeID, _profileOnly);

            if (IsValidSupplementalData(table))
                SaveSupplementalData(table);
        }

        /// <summary>
    /// Saves data from DataGridSupplemental.
    /// </summary>
    /// <param name="table">The table holding the grid data.</param>
    /// <returns>True if OK; else False.</returns>
        private bool SaveSupplementalData(DataTable table)
        {
            DataGridItem item = default(DataGridItem);
            TextBox tbFundingValue = default(TextBox);
            CheckBox cbFundingValue = default(CheckBox);
            DropDownList ddlFundingValue = default(DropDownList);
            DataRow row = default(DataRow);
            StringBuilder sb = new StringBuilder();
            string name = default(string);
            string value = default(string);
            string dataType = default(string);
            string lookupTableName = default(string);

            {
                var withBlock = DataGridSupplemental;
                var loopTo = withBlock.Items.Count - 1;
                for (int index = 0; index <= loopTo; index++)
                {
                    item = withBlock.Items[index];
                    row = table.Rows[index];

                    tbFundingValue = (TextBox)item.Cells[(int)gridColumnsSupplemental.Value].FindControl("TextBoxFundingValue");
                    cbFundingValue = (CheckBox)item.Cells[(int)gridColumnsSupplemental.Value].FindControl("CheckBoxFundingValue");
                    ddlFundingValue = (DropDownList)item.Cells[(int)gridColumnsSupplemental.Value].FindControl("DropDownListFundingValue");

                    name = ((Label)item.Cells[(int)gridColumnsSupplemental.EligibilityName].FindControl("LabelName")).Text.Trim();
                    dataType = row.ToString("DataType");

                    if (tbFundingValue.Visible)
                    {
                        value = tbFundingValue.Text.Trim();

                        if (value != string.Empty)
                            _dataComponent.AddProfileSetupSingle("CLI", _clientID, name, value, string.Empty, dataType, _clientDataTypeID, Session["UserEmailAddress"].ToString());
                        else
                            _dataComponent.DeleteProfileSetupSingle("CLI", _clientID, name, _clientDataTypeID);
                    }
                    else if (cbFundingValue.Visible)
                    {
                        if (cbFundingValue.Checked)
                            _dataComponent.AddProfileSetupSingle("CLI", _clientID, name, "1", string.Empty, dataType, _clientDataTypeID, Session["UserEmailAddress"].ToString());
                        else
                            _dataComponent.DeleteProfileSetupSingle("CLI", _clientID, name, _clientDataTypeID);
                    }
                    else if (ddlFundingValue.Visible)
                    {
                        if (ddlFundingValue.SelectedIndex > 0)
                        {
                            lookupTableName = row.ToString("LookupTableName");
                            _dataComponent.AddProfileSetupSingle("CLI", _clientID, name, ddlFundingValue.SelectedItem.Value, lookupTableName, dataType, _clientDataTypeID, Session["UserEmailAddress"].ToString());
                        }
                        else
                            _dataComponent.DeleteProfileSetupSingle("CLI", _clientID, name, _clientDataTypeID);
                    }
                }
            }

            ShowMessage(LabelErrorSupplemental, "Data saved.", true);
            return default(bool);
        }

        /// <summary>
    /// Validates the DataGridSupplemental data.
    /// </summary>
    /// <param name="table">A DataTable containing the original supplemental data.</param>
    /// <returns>True if data is valid; else False.</returns>
        private bool IsValidSupplementalData(DataTable table)
        {
            DataGridItem item = default(DataGridItem);
            TextBox tbFundingValue = default(TextBox);
            CheckBox cbFundingValue = default(CheckBox);
            DropDownList ddlFundingValue = default(DropDownList);
            DataRow row = default(DataRow);
            StringBuilder sb = new StringBuilder();
            string name = default(string);
            string value = default(string);
            string dataType = default(string);
            decimal resultDecimal;
            DateTime resultDatetime;

            {
                var withBlock = DataGridSupplemental;
                var loopTo = withBlock.Items.Count - 1;
                for (int index = 0; index <= loopTo; index++)
                {
                    item = withBlock.Items[index];
                    row = table.Rows[index];

                    tbFundingValue = (TextBox)item.Cells[(int)gridColumnsSupplemental.Value].FindControl("TextBoxFundingValue");
                    cbFundingValue = (CheckBox)item.Cells[(int)gridColumnsSupplemental.Value].FindControl("CheckBoxFundingValue");
                    ddlFundingValue = (DropDownList)item.Cells[(int)gridColumnsSupplemental.Value].FindControl("DropDownListFundingValue");

                    if (tbFundingValue.Visible)
                    {
                        value = tbFundingValue.Text.Trim();
                        name = ((Label)item.Cells[(int)gridColumnsSupplemental.EligibilityName].FindControl("LabelName")).Text.Trim();
                        dataType = row.ToString("DataType");

                        if (value != string.Empty)
                        {
                            switch (dataType)
                            {
                                case "N":
                                    {
                                        if (decimal.TryParse(value, out resultDecimal) == false)
                                            sb.AddMessage(string.Format("{0} must have a valid numeric value.", name));
                                        break;
                                    }

                                case "D":
                                    {
                                        if (DateTime.TryParse(value, out resultDatetime) == false)
                                            sb.AddMessage(string.Format("{0} must have a valid date value (mm/dd/yyyy).", name));
                                        break;
                                    }
                            }
                        }
                    }
                    else if (cbFundingValue.Visible)
                    {
                    }
                    else if (ddlFundingValue.Visible)
                    {
                    }
                }
            }

            if (sb.Length == 0)
                return true;
            else
            {
                ShowMessage(LabelErrorSupplemental, sb.ToString());
                return false;
            }
        }

        /// <summary>
    /// Handles the LinkButtonReturnToProfileSetup Click event. It transfers control back to the client screen.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void LinkButtonReturnToProfileSetup_Click(object sender, System.EventArgs e)
        {
            // Server.Transfer("ProfileSetup.aspx")
            Response.Redirect("ProfileSetup.aspx");
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
