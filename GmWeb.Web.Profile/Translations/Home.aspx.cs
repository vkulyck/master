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
    partial class Home : System.Web.UI.Page
    {
        // Empty ASP controls to fix immediate build issues
        Label
            LabelBirthday, LabelGender, LabelEthnicity, LabelCulturalIdentityID,
            LabelLanguageIDPrimary, LabelLanguageIDSecondary, LabelEmail,
            LabelPhone, LabelAddress, LabelAddressLine2
        ;
        //********************************************//

        private DataComponent _dataComponent;
        public int ClntID;
        public string ClientFullName;
        public int AgencyIDParent;



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

            if (!IsPostBack)
            {
            }

            // ' Gets a reference to a Label control from Master page
            // Dim mpLabel As Label
            // mpLabel = CType(Master.FindControl("UserFullName"), Label)
            // If Not mpLabel Is Nothing Then
            // mpLabel.Text = CStr(Session("FirstName")) + " " + CStr(Session("LastName"))
            // End If

            ClientFullName = System.Convert.ToString(Session["FirstName"]) + " " + System.Convert.ToString(Session["LastName"]);
            ClntID = System.Convert.ToInt32(Session["ClientID"]);
            AgencyIDParent = System.Convert.ToInt32(Session["AgencyIDParent"]);
            GetVolunteer();
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
    }
}
