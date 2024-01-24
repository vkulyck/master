using System;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;

namespace GmWeb.Web.ProfileApp
{
    partial class Volunteer : System.Web.UI.MasterPage
    {
        // Empty ASP controls to fix immediate build issues
        Label LabelCurrentYear;
        Button ButtonHome, ButtonServices, ButtonVolunteer, ButtonDonor, ButtonProfile;
        CheckBox CheckBoxChecked;
        TextBox TextBoxDocumentComments;
        DropDownList DropDownListService, DropDownListProcessType;
        DataGrid DataGridProcessStep;
        FileUpload FileUploadDocuments;
        //********************************************//

        public int _clientID = 0;


        /// <summary>
    /// Handles the Page Load event. It sets controls on the page.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            LabelCurrentYear.Text = DateTime.Now.Year.ToString();
        }

        /// <summary>
    /// Handles the Page PreRender event. It sets the button controls' format.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            SetDefaultColors();

            var path = Request.AppRelativeCurrentExecutionFilePath;
            switch (path.Substring(2).ToLower())
            {
                case "home.aspx":
                    {
                        ButtonHome.BackColor = Color.FromArgb(0xB7D8ED);
                        ButtonHome.ForeColor = Color.FromArgb(0x6699);
                        ButtonHome.Font.Italic = true;
                        break;
                    }

                case "services.aspx":
                    {
                        ButtonServices.BackColor = Color.FromArgb(0xB7D8ED);
                        ButtonServices.ForeColor = Color.FromArgb(0x6699);
                        ButtonServices.Font.Italic = true;
                        break;
                    }

                case "volunteer.aspx":
                    {
                        ButtonVolunteer.BackColor = Color.FromArgb(0xB7D8ED);
                        ButtonVolunteer.ForeColor = Color.FromArgb(0x6699);
                        ButtonVolunteer.Font.Italic = true;
                        break;
                    }

                case "donor.aspx":
                    {
                        ButtonDonor.BackColor = Color.FromArgb(0xB7D8ED);
                        ButtonDonor.ForeColor = Color.FromArgb(0x6699);
                        ButtonDonor.Font.Italic = true;
                        break;
                    }

                case "profile.aspx":
                    {
                        ButtonProfile.BackColor = Color.FromArgb(0xB7D8ED);
                        ButtonProfile.ForeColor = Color.FromArgb(0x6699);
                        ButtonProfile.Font.Italic = true;
                        break;
                    }
            }
        }

        /// <summary>
    /// Handles the ButtonHome Click event. It sets the button foreground and background colors, then transfers to Home.axpx.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonHome_Click(object sender, System.EventArgs e)
        {
            // Server.Transfer("Home.aspx")
            Response.Redirect("Home.aspx");
        }

        /// <summary>
    /// Handles the ButtonHome Click event. It sets the button foreground and background colors, then transfers to Home.axpx.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonServices_Click(object sender, System.EventArgs e)
        {
            // Server.Transfer("Services.aspx")
            Response.Redirect("Services.aspx");
        }

        /// <summary>
    /// Handles the ButtonHome Click event. It sets the button foreground and background colors, then transfers to Home.axpx.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonVolunteer_Click(object sender, System.EventArgs e)
        {
            // Server.Transfer("Volunteer.aspx")
            Response.Redirect("Volunteer.aspx");
        }

        /// <summary>
    /// Handles the ButtonHome Click event. It sets the button foreground and background colors, then transfers to Home.axpx.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonDonor_Click(object sender, System.EventArgs e)
        {
            // Server.Transfer("Donor.aspx")
            Response.Redirect("Donor.aspx");
        }

        /// <summary>
    /// Handles the ButtonHome Click event. It sets the button foreground and background colors, then transfers to Home.axpx.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonProfile_Click(object sender, System.EventArgs e)
        {
            // Server.Transfer("Profile.aspx")
            Response.Redirect("Profile.aspx");
        }

        /// <summary>
    /// Sets the default foreground and background colors for all buttons.
    /// </summary>
        private void SetDefaultColors()
        {
            ButtonHome.BackColor = Color.FromArgb(0x67ADE4);
            ButtonHome.ForeColor = Color.White;
            ButtonHome.Font.Italic = false;

            ButtonServices.BackColor = Color.FromArgb(0x67ADE4);
            ButtonServices.ForeColor = Color.White;
            ButtonServices.Font.Italic = false;

            ButtonVolunteer.BackColor = Color.FromArgb(0x67ADE4);
            ButtonVolunteer.ForeColor = Color.White;
            ButtonVolunteer.Font.Italic = false;

            ButtonDonor.BackColor = Color.FromArgb(0x67ADE4);
            ButtonDonor.ForeColor = Color.White;
            ButtonDonor.Font.Italic = false;

            ButtonProfile.BackColor = Color.FromArgb(0x67ADE4);
            ButtonProfile.ForeColor = Color.White;
            ButtonProfile.Font.Italic = false;
        }

        protected void lnkLogOut_Click(object sender, System.EventArgs e)
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.Redirect("Logon.aspx");
        }

        protected void LinkButtonLogout_Click(object sender, System.EventArgs e)
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.Redirect("Logon.aspx");
        }
    }
}
