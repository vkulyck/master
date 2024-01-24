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
using GmWeb.Web.ProfileApp;

namespace GmWeb.Web.ProfileApp
{
    partial class Ajax : System.Web.UI.Page
    {

        // Declare the database data component object.

        private DataComponent _dataComponent;

        private void Page_Load(System.Object sender, System.EventArgs e)
        {
            // Transfer to the logon screen if they're not logged in.
            if (Session.Count == 0 || Session["Email"] == null)
                Response.Redirect("Logon.aspx");

            // Instantiate the database data component object.
            _dataComponent = new DataComponent();

            if (IsPostBack == false)
                UpdateClientCategoryDateSignature();
        }

        /// <summary>
    /// Update client's signature for attendance
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
        private bool UpdateClientCategoryDateSignature()
        {
            int clientCategoryDateID = System.Convert.ToInt32(Request.Form["ClientCategoryDateID"]);
            string signature = Request.Form["Signature"];

            if (clientCategoryDateID > 0)
            {
                _dataComponent.UpdateClientCategoryDateSignature(clientCategoryDateID, signature);
                return true;
            }
            return false;
        }
    }
}
