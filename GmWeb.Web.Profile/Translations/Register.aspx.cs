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
    partial class Register : System.Web.UI.Page
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

        DropDownList DropDownListStreetType;

        Panel PanelSearchKeywords, PanelSearchProfiles, PanelServices, PanelMatchServices, PanelConsent;

        RadioButton RadioButtonSearchByProfiles, RadioButtonSearchByKeywords;

        Button ButtonPrevious, ButtonNext;

        DataGrid DataGridProfiles, DataGridSelectedServices, DataGridServices, DataGridConsent;
        TextBox 
            TextBoxAgencyName, TextBoxUserEmailAddress, TextBoxUserPassword, TextBoxPassword, TextBoxFirstName, TextBoxLastName,
            TextBoxAddressNo, TextBoxAddressStreet, TextBoxAddressLine2, TextBoxCity, TextBoxZip, TextBoxPhone,
            TextBoxReEnterPassword
        ;

        //********************************************//

        private DataComponent _dataComponent;



        /// <summary>
    /// Handles the Page Load event. It instantiates a DataComponent object, then sets controls on the page.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            _dataComponent = new DataComponent();
            LabelError.Visible = false;

            if (!IsPostBack)
                LoadDDLs();
        }

        /// <summary>
    /// Loads the DDLs on this screen.
    /// </summary>
        private void LoadDDLs()
        {
            {
                var withBlock = DropDownListStreetType;
                withBlock.DataSource = _dataComponent.GetStreetTypes();
                withBlock.DataBind();
            }
        }

        /// <summary>
    /// Handles the ButtonSubmit Click event. It validates and saves the volunteer's data.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonSubmit_Click(object sender, System.EventArgs e)
        {
            int agencyID = 0;
            string agencyName = TextBoxAgencyName.Text.Trim();
            if (agencyName != string.Empty)
                TextBoxAgencyName.ToolTip = agencyName;
            int locOpen = agencyName.LastIndexOf("(");
            int locClose = agencyName.LastIndexOf(")");

            if (locOpen >= 0 && locClose >= 0)
            {
                agencyID = int.Parse(agencyName.Substring(locOpen + 1, locClose - locOpen - 1));
                Session["AgencyID"] = agencyID;
            }
            else
                Session["AgencyID"] = null;

            if (IsValidData())
            {
                int clientID = _dataComponent.SaveVolunteerData(TextBoxUserEmailAddress.Text.Trim(), TextBoxPassword.Text.Trim(), agencyID, TextBoxLastName.Text.Trim(), TextBoxFirstName.Text.Trim(), TextBoxAddressNo.Text.Trim(), TextBoxAddressStreet.Text.Trim(), DropDownListStreetType.SelectedItem.Value, TextBoxAddressLine2.Text.Trim(), TextBoxCity.Text.Trim(), TextBoxZip.Text.Trim(), TextBoxPhone.Text.Trim()
                     );

                if (clientID == _dataComponent.DUPLICATE_EMAIL)
                {
                    ShowMessage("The email address entered is already being used.");
                    return;
                }

                Session["ClientID"] = clientID;
                Session["AgencyName"] = TextBoxAgencyName.Text;
                Session["FirstName"] = TextBoxFirstName.Text.Trim();
                Session["LastName"] = TextBoxLastName.Text.Trim();

                Server.Transfer("Home.aspx");
            }
        }

        /// <summary>
    /// Validates the data on the registration screen.
    /// </summary>
    /// <returns>True if valid; else False.</returns>
        private bool IsValidData()
        {
            StringBuilder sb = new StringBuilder();

            if (TextBoxUserEmailAddress.Text.Trim() == string.Empty)
                sb.AddMessage("Email address is required.");
            else if (Utility.IsValidEmail(TextBoxUserEmailAddress.Text.Trim()) == false)
                sb.AddMessage("Invalid email address.");

            if (TextBoxPassword.Text.Trim() == string.Empty)
                sb.AddMessage("Password is required.");
            if (TextBoxPassword.Text.Trim() != TextBoxReEnterPassword.Text.Trim())
                sb.AddMessage("The password values entered are not the same.");
            if (Session["AgencyID"] == null)
                sb.AddMessage("Please select a participating agency.");
            if (TextBoxLastName.Text.Trim() == string.Empty)
                sb.AddMessage("Last name is required.");
            if (TextBoxFirstName.Text.Trim() == string.Empty)
                sb.AddMessage("First name is required.");

            if (TextBoxAddressNo.Text.Trim() == string.Empty)
                sb.AddMessage("Address number is required.");
            if (TextBoxAddressStreet.Text.Trim() == string.Empty)
                sb.AddMessage("Street is required.");
            if (DropDownListStreetType.SelectedIndex <= 0)
                sb.AddMessage("Please select a street type.");
            if (TextBoxCity.Text.Trim() == string.Empty)
                sb.AddMessage("City is required.");

            if (TextBoxZip.Text.Trim() == string.Empty)
                sb.AddMessage("Zip code is required.");
            else if (Utility.IsValidZip(TextBoxZip.Text.Trim()) == false)
                sb.AddMessage("Invalid zip code.");

            if (TextBoxPhone.Text.Trim() == string.Empty)
                sb.AddMessage("Phone is required.");
            else if (Utility.IsValidPhone(TextBoxPhone.Text.Trim()) == false)
                sb.AddMessage("Phone number should be in format xxx-xxx-xxxx.");

            if (sb.Length == 0)
                return true;
            else
            {
                ShowMessage(sb.ToString());
                return false;
            }
        }

        /// <summary>
    /// Displays the message in red if it's an error or blue if it's a message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="isMessage">True if message; else False.</param>
        private void ShowMessage(string message, bool isMessage = false)
        {
            var messageColor = isMessage ? Color.FromArgb(unchecked((int)0xFF0060A0)) : Color.Red;

            LabelError.Visible = true;
            LabelError.ForeColor = messageColor;
            LabelError.Text = message;
        }



        /// <summary>
    /// This is a web service used to supply a list of agency names.
    /// </summary>
    /// <param name="prefixText">The first few characters of the LastName, FirstName, as entered in TextBoxOccupant.</param>
    /// <param name="count">The number of confirmation numbers to return.</param>
    /// <returns>A string array with a list of {count} number of confirmation numbers and OnlineAppDataID, whose confirmation
    /// number starts with prefixText.
    /// </returns>
        [System.Web.Services.WebMethod()]
        [System.Web.Script.Services.ScriptMethod()]
        public static string[] GetAgencyNames(string prefixText, int count)
        {
            AjaxData ajaxData = new AjaxData();
            return ajaxData.GetAgencyNames(prefixText, count);
        }
    }
}
