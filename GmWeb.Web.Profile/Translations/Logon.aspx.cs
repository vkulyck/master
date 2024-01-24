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

namespace GmWeb.Web.ProfileApp
{
    partial class Logon : System.Web.UI.Page
    {
        // Empty ASP controls to fix immediate build issues
        Label LabelErrorLogon, LabelCurrentYear, TextBoxUserEmailAddress, AgencyName;
        Image AgencyImageFileName;
        TextBox TextBoxPassword;
        //********************************************//

        // Declare the database data component object.
        private DataComponent _dataComponent;

        private const string _subject = "Auto-Message from Good Mojo";
        private const string _message = "This e-mail is in response to your clicking on the Forgot Password link on the GmWeb Sign-in screen. Your user information is as follows:<br>Email: {0}<br>Password: {1}<br><br>Please keep this information confidential and secure.";

        /// <summary>
    /// Load the page. Set the page ID; load the controls and set their visibility depending on the
    /// approval level; set the ImageButton rollover attributes.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>a
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Transfer to the home screen if they're logged in.
            if (!(Session["Email"] == null))
                Response.Redirect("Home.aspx");

            // Instantiate the database data component object.
            _dataComponent = new DataComponent();
            LabelErrorLogon.Visible = false;

            if (IsPostBack == false)
            {
                Session.Timeout = 90;

                // ImageButtonEnter.Attributes.Add("onmouseover", "changeOverButton(this, 'login')")
                // ImageButtonEnter.Attributes.Add("onmouseout", "changeOutButton(this, 'login')")

                LabelCurrentYear.Text = DateTime.Today.ToString("yyyy");
                TextBoxUserEmailAddress.Focus();
            }

            AgencyName.Text = "GoodMojo";
            AgencyImageFileName.ImageUrl = "./images/goodmojo-logo.png";
            AgencyImageFileName.Attributes.Add("style", "max-width: 100px");
        }

        // ''' <summary>
        // ''' Handles the ImageButtonEnter click event. It directs the user to the home screen.
        // ''' </summary>
        // ''' <param name="sender">The object which initiated the event.</param>
        // ''' <param name="e">The arguments associated with the event.</param>
        // Protected Sub ImageButtonEnter_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonEnter.Click
        // If IsValidUser() Then
        // 'Server.Transfer("Home.aspx")
        // Response.Redirect("Home.aspx")
        // End If
        // End Sub

        /// <summary>
    /// Processes the BtnLogin click event. If the email address and password credentials are valid, transfers
    /// the user to ExistingAgencyInfo.aspx.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void BtnLogin_Click(object sender, System.EventArgs e)
        {
            if (IsValidUser())
                Response.Redirect("Home.aspx");
        }

        /// <summary>
    /// Validates that the email and password are both valid, that the user has a sufficient approval level,
    /// then sets the initial session state with the information available so far.
    /// </summary>
    /// <returns>True if the user's credentials are valid; else it returns False.</returns>
        private bool IsValidUser()
        {
            // Fetch the user's information.
            DataTable tableUser = _dataComponent.GetVolunteer(TextBoxUserEmailAddress.Text.Trim());

            // Check if any user matched the value entered.
            if (tableUser.Rows.Count > 0)
            {
                // Extract the user's data into a DataRow.
                DataRow rowUser = tableUser.Rows[0];
                string password = System.Convert.ToString(rowUser["Password"]).Trim();

                if (TextBoxPassword.Text.Trim() != password)
                {
                    LabelErrorLogon.Visible = true;
                    LabelErrorLogon.ForeColor = System.Drawing.Color.Red;
                    LabelErrorLogon.Text = "Invalid login.";
                    return false;
                }

                // Save relevant data to session variables.
                Session["ClientID"] = rowUser.ToString("ClientID");
                Session["AgencyName"] = rowUser.ToString("AgencyName");
                Session["FirstName"] = rowUser.ToString("FirstName");
                Session["LastName"] = rowUser.ToString("LastName");
                Session["Email"] = rowUser.ToString("Email");
                Session["UserEmailAddress"] = rowUser.ToString("Email");

                int agencyID = rowUser.ToInteger("AgencyID");
                int agencyIDParent = _dataComponent.GetParentAgencyID(agencyID);
                Session["AgencyID"] = rowUser.ToString("AgencyID");
                Session["AgencyIDParent"] = agencyIDParent;

                return true;
            }
            else
            {
                // No userEmailAddress matched the value entered. Return an error.
                LabelErrorLogon.Visible = true;
                LabelErrorLogon.ForeColor = System.Drawing.Color.Red;
                LabelErrorLogon.Text = "Invalid login.";
                return false;
            }
        }
    }
}
