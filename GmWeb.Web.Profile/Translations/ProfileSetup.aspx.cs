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
    partial class ProfileSetup : System.Web.UI.Page
    {
        // Empty ASP controls to fix immediate build issues
        Label LabelErrorBasicInfo, LabelCurrentYear, TextBoxUserEmailAddress, AgencyName, LabelBirthdayReadOnly, LabelGenderReadOnly;
        DropDownList DropDownListStreetType, DropDownListGender, DropDownListProfiles;
        TextBox 
            TextBoxPassword, TextBoxFirstName, TextBoxLastName, TextBoxAddressNo, TextBoxAddressStreet, 
            TextBoxAddressLine2, TextBoxCity, TextBoxZip, TextBoxPhone, TextBoxBirthday
        ;
        CheckBox CheckBoxThisClientLivesOnPremises;
        RadioButton RadioButtonShortTermShelter, RadioButtonLongTermShelter, RadioButtonHomelessPrevention;
        //********************************************//

        private DataComponent _dataComponent;
        private int _clientID = 0;
        public string ClientFullName;

        /// <summary>
        /// Handles the Page Load event. It instantiates a DataComponent object, then sets controls on the page.
        /// </summary>
        /// <param name="sender">The object which initiated the event.</param>
        /// <param name="e">The arguments associated with the event.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Transfer to the logon screen if they're not logged in.
            if (Session.Count == 0 || Session["Email"] == null)
                Response.Redirect("Logon.aspx");

            _dataComponent = new DataComponent();
            _clientID = int.Parse(Session["ClientID"].ToString());
            ClientFullName = System.Convert.ToString(Session["FirstName"]) + " " + System.Convert.ToString(Session["LastName"]);

            LabelErrorBasicInfo.Visible = false;

            if (!IsPostBack)
            {
                BindDDLs();
                LoadBasicInfo();
            }
        }



        /// <summary>
    /// Binds the page's DDLs.
    /// </summary>
        private void BindDDLs()
        {
            {
                var withBlock = DropDownListStreetType;
                withBlock.DataSource = _dataComponent.GetStreetTypes();
                withBlock.DataBind();
                withBlock.SelectedIndex = 0;
            }

            {
                var withBlock1 = DropDownListGender;
                withBlock1.DataSource = _dataComponent.GetGender();
                withBlock1.DataBind();
                withBlock1.SelectedIndex = 0;
            }

            {
                var withBlock2 = DropDownListProfiles;
                withBlock2.DataSource = _dataComponent.GetClientDataTypes();
                withBlock2.DataBind();
                withBlock2.SelectedIndex = 0;
            }
        }

        /// <summary>
    /// Loads the client data controls for the current clientID.
    /// </summary>
        private void LoadBasicInfo()
        {
            DataTable table = _dataComponent.GetClient(_clientID);
            DataRow row = table.Rows[0];

            TextBoxFirstName.Text = row.ToString("FirstName");
            TextBoxLastName.Text = row.ToString("LastName");
            TextBoxAddressNo.Text = row.ToString("AddressNo");
            TextBoxAddressStreet.Text = row.ToString("AddressStreet");
            DropDownListStreetType.SetValue(row, "AddressStreetType");

            TextBoxAddressLine2.Text = row.ToString("AddressLine2");
            TextBoxCity.Text = row.ToString("City");
            TextBoxZip.Text = row.ToString("Zip");
            TextBoxPhone.Text = row.ToString("Phone");
            TextBoxBirthday.Text = row.ToString("Birthday");
            LabelBirthdayReadOnly.Text = row.ToString("Birthday");

            int shelterType = row.ToInteger("ShelterType");
            RadioButtonShortTermShelter.Checked = (shelterType == Utility.shortTermShelter) ? true : false;
            RadioButtonLongTermShelter.Checked = (shelterType == Utility.longTermShelter) ? true : false;
            RadioButtonHomelessPrevention.Checked = (shelterType == Utility.homelessPrevention) ? true : false;
            DropDownListGender.SetValue(row, "Gender");
            LabelGenderReadOnly.Text = row.ToString("Gender").Replace("M", "Male").Replace("F", "Female").Replace("T", "Transgender").Replace("U", "Unidentified");
        }

        /// <summary>
    /// Handles the ButtonSubmitBasicInfo Click event. It validates and saves the client data.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonSubmitBasicInfo_Click(object sender, System.EventArgs e)
        {
            if (IsValidDetail())
            {
                int shelterType = Utility.noShelter;

                if (RadioButtonShortTermShelter.Checked)
                    shelterType = Utility.shortTermShelter;
                if (RadioButtonLongTermShelter.Checked)
                    shelterType = Utility.longTermShelter;
                if (RadioButtonHomelessPrevention.Checked)
                    shelterType = Utility.homelessPrevention;
                DateTime birthday = (TextBoxBirthday.Text.Trim() == string.Empty) ? Utility.NullDate : Convert.ToDateTime(TextBoxBirthday.Text.Trim());

                _dataComponent.UpdateClientDataShort(_clientID, int.Parse(Session["AgencyID"].ToString()), DropDownListStreetType.SelectedItem.Value, birthday, DropDownListGender.SelectedItem.Value, TextBoxZip.Text.Trim(), shelterType, TextBoxAddressLine2.Text.Trim(), TextBoxAddressNo.Text.Trim(), TextBoxAddressStreet.Text.Trim(), TextBoxCity.Text.Trim(), TextBoxFirstName.Text.Trim(), TextBoxLastName.Text.Trim(), TextBoxPhone.Text.Trim(), Session["UserEmailAddress"].ToString(), CheckBoxThisClientLivesOnPremises.Checked
                   );

                ClientFullName = TextBoxFirstName.Text.Trim() + " " + TextBoxLastName.Text.Trim();
                LabelBirthdayReadOnly.Text = birthday.ToShortDateString();
                LabelGenderReadOnly.Text = DropDownListGender.SelectedItem.Value.Replace("M", "Male").Replace("F", "Female").Replace("T", "Transgender").Replace("U", "Unidentified");

                ShowMessage(LabelErrorBasicInfo, "Data saved.", true);
            }
        }

        /// <summary>
    /// Validates the client data controls.
    /// </summary>
    /// <returns>True if valid; else False.</returns>
        private bool IsValidDetail()
        {
            StringBuilder sb = new StringBuilder();
            DateTime dateResult = default(DateTime);
            int integerResult = 0;

            if (TextBoxFirstName.Text.Trim() == string.Empty)
                sb.AddMessage("First Name is required.");
            if (TextBoxLastName.Text.Trim() == string.Empty)
                sb.AddMessage("Last Name is required.");

            if (TextBoxZip.Text.Trim() != string.Empty)
            {
                if (Utility.IsValidZip(TextBoxZip.Text.Trim()) == false)
                    sb.AddMessage("Zip is invalid.");
            }

            if (TextBoxPhone.Text.Trim() != string.Empty)
            {
                if (Utility.IsValidPhone(TextBoxPhone.Text.Trim()) == false)
                    sb.AddMessage("Phone is invalid.");
            }

            if (TextBoxBirthday.Text.Trim() != string.Empty)
            {
                if (DateTime.TryParse(TextBoxBirthday.Text, out dateResult) == false)
                    sb.AddMessage("Invalid birthday.");
            }

            if (sb.Length == 0)
                return true;
            else
            {
                ShowMessage(LabelErrorBasicInfo, sb.ToString());
                return false;
            }
        }



        /// <summary>
    /// Handles the DropDownListProfiles SelectedIndexChanged event. It sets up the profile dialog.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DropDownListProfiles_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Transfer to the Client Supplemental screen with appropriate parameters.
            int clientDataTypeID = int.Parse(DropDownListProfiles.SelectedItem.Value);
            // Server.Transfer(String.Format("ClientSupplementalData.aspx?ClientID={0}&ClientDataTypeID={1}&ProfileOnly=Y", _clientID, clientDataTypeID))
            Response.Redirect(string.Format("ClientSupplementalData.aspx?ClientID={0}&ClientDataTypeID={1}&ProfileOnly=Y", _clientID, clientDataTypeID));
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

        /// <summary>
    /// This routine returns the boolean value of the value argument; it returns False if value is Nothing.
    /// </summary>
    /// <param name="value">The object to be converted to boolean.</param>
    /// <returns>The boolean value of the value argument; it returns False if value is Nothing.</returns>
        public bool GetChecked(object value)
        {
            return Utility.GetChecked(value);
        }
    }
}
