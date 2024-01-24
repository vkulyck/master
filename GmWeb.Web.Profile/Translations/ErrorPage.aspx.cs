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

    /// <summary>

/// All unhandled errors are routed to this page. See the Application_Error routine in Global.asax, which

/// does the routing to this page.

/// </summary>
    partial class ErrorPage : System.Web.UI.Page
    {
        // Empty ASP controls to fix immediate build issues
        Label LabelErrorDescription, LabelStackTrace;
        //********************************************//

        // Declare the database data component object.
        private DataComponent _dataComponent;

        /// <summary>
    /// Present a formatted page describing the error. The top part of the page informs the user of the
    /// error and gives a phone number to call. The bottom part shows the stack trace.
    /// </summary>
    /// <param name="sender">The object which initiated the page load.</param>
    /// <param name="e">The event arguments.</param>
        private void Page_Load(System.Object sender, System.EventArgs e)
        {
            // Instantiate the database data component object.
            _dataComponent = new DataComponent();

            // Handle the exception.
            try
            {
                // Get the latest exception. If there is none, just exit.
                Exception exc = Server.GetLastError();
                if (exc == null)
                    return;

                Exception excBase = exc.GetBaseException();
                if (excBase == null)
                    return;

                // Set the error description and stack trace.
                string lineBreak = "<br>";

                LabelErrorDescription.Text = excBase.Message;

                if (excBase.StackTrace != null)
                {
                    string stackTrace = excBase.StackTrace.Trim();
                    stackTrace = stackTrace.Replace("at ", lineBreak);

                    if (stackTrace.StartsWith(lineBreak))
                        stackTrace = stackTrace.Substring(lineBreak.Length);

                    LabelStackTrace.Text = stackTrace;
                }
                else
                    LabelStackTrace.Text = "No stack trace available";

                // Log the error.
                _dataComponent.LogError(excBase.Source, excBase.Message + global::Utility.Newline + LabelStackTrace.Text);
            }
            finally
            {
                // Always clear the exception and all server variables.
                Server.ClearError();
                Session.Clear();
            }
        }
    }
}
