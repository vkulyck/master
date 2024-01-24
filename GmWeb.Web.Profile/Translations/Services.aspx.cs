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
using GmWeb.Logic.Utility.Extensions;
using System.IO;


namespace GmWeb.Web.ProfileApp
{
    partial class Services : System.Web.UI.Page
    {
        // Empty ASP controls to fix immediate build issues
        Label LabelError, LabelErrorStep, LabelProcessTypeValue, LabelDescription, LabelDateRecorded, LabelDocumentPath;
        CheckBox CheckBoxChecked;
        TextBox TextBoxDocumentComments;
        DropDownList DropDownListService, DropDownListProcessType;
        DataGrid DataGridProcessStep;
        FileUpload FileUploadDocuments;
        //********************************************//

        private DataComponent _dataComponent;
        private int _clientID = 0;
        private Dictionary<int, string> _entityTableList;
        private Dictionary<string, bool> _tablePassList;

        private const string PDF_FILE = ".pdf";

        private enum processStepGridColumns
        {
            Select,
            Action,
            ProcessType,
            Description,
            Entity,
            Checked,
            Document,
            DateDocumentRecorded,
            DocumentComments,
            GroupNumber,
            Sequence,
            SkipNextStepsIfPassed,
            EntityID,
            ProcessActionID
        }

        private enum eProcessAction
        {
            NoAction,
            ClientQualified,
            RegisteredForService = 3,
            ApplicationCompleted,
            SubmittedApplication,
            DocumentSigned,
            DocumentUploaded,
            ApplicationApproved,
            PaymentApproved,
            DocIssuedGenerated,
            IncompleteStep,
            ServicePaymentMade,
            AttendedService,
            ServiceSigRequired,
            Test = 9999
        }

        private enum eTableNumber
        {
            lkpStaff,
            tblAgency,
            tblClient,
            tblClientAssociation,
            tblClientJob,
            tblClientOccupant
        }



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
            LabelError.Visible = false;
            LabelErrorStep.Visible = false;
            _clientID = int.Parse(Session["ClientID"].ToString());

            if (!IsPostBack)
            {
                ViewState["CurrentGroupNumber"] = 0;
                BindServices(_clientID);
                ActionNeededCheck();
            }
        }

        /// <summary>
    /// Load service based on query parameter
    /// </summary>
    /// <remarks></remarks>
        private void ActionNeededCheck()
        {
            if (Request.QueryString.HasKeys() && Request.QueryString["clientCategoryID"] != null)
            {
                DropDownListService.SelectedValue = Request.QueryString["clientCategoryID"];
                LoadProcessTypes(System.Convert.ToInt32(Request.QueryString["clientCategoryID"]));

                {
                    var withBlock = DataGridProcessStep;
                    withBlock.DataSource = null;
                    withBlock.DataBind();
                }

                // Open first process type
                DropDownListProcessType.SelectedIndex = 1;
                BindProcessStep();
            }
        }

        /// <summary>
    /// Loads DropDownListProcessType.
    /// </summary>
        private void LoadProcessTypes(int clientCategoryID)
        {
            {
                var withBlock = DropDownListProcessType;
                withBlock.DataSource = _dataComponent.GetProcessTypes(clientCategoryID, Convert.ToInt32(Session["AgencyIDParent"]));
                withBlock.DataBind();

                ListItem item = new ListItem(" (Select)", "0");
                withBlock.Items.Insert(0, item);
            }
        }

        /// <summary>
    /// Handles the DropDownListService SelectedIndexChanged event. It loads DropDownListProcessType for a given clientCategoryID.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DropDownListService_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int clientCategoryID = int.Parse(DropDownListService.SelectedItem.Value);
            LoadProcessTypes(clientCategoryID);

            {
                var withBlock = DataGridProcessStep;
                withBlock.DataSource = null;
                withBlock.DataBind();
            }
        }

        /// <summary>
    /// Binds DropDownListService.
    /// </summary>
    /// <param name="clientID">Client ID.</param>
        private void BindServices(int clientID)
        {
            {
                var withBlock = DropDownListService;
                withBlock.DataSource = _dataComponent.GetClientServices(clientID);
                withBlock.DataBind();
            }

            if (DropDownListProcessType.Items.Count > 0)
                DropDownListProcessType.SelectedIndex = 0;
            {
                var withBlock1 = DataGridProcessStep;
                withBlock1.DataSource = null;
                withBlock1.DataBind();
            }
        }

        /// <summary>
    /// Handles the DropDownListProcessType SelectedIndexChanged event. It loads DataGridProcessStep.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DropDownListProcessType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            BindProcessStep();
        }

        /// <summary>
    /// Loads DataGridProcessStep.
    /// </summary>
        private void BindProcessStep()
        {
            const long CURRENT_GROUP_COLOR = (long)0xD6E0F5; // blue
            const long COMPLETED_GROUP_COLOR = (long)0xD6F5D6; // green
            const long UNMARKED_COLOR = (long)0xFFFFFF; // white

            int processTypeID = int.Parse(DropDownListProcessType.SelectedItem.Value);
            int clientCategoryID = int.Parse(DropDownListService.SelectedItem.Value);
            int currentGroupNumber = Convert.ToInt32(ViewState["CurrentGroupNumber"]);
            DataGridItem item = null;
            bool showLinkText = false;
            int groupNumber = 0;
            int prevGroupNumber = -1;
            bool isCompleteGroup = false;
            string tableName = string.Empty;
            int entityID = 0;
            int processActionID = 0;
            LinkButton lbSelect = null;
            LinkButton lbAction = null;

            DataTable table = _dataComponent.GetProcessSteps(processTypeID, clientCategoryID, Convert.ToInt32(Session["AgencyIDParent"]));
            SetupDictionaries();

            {
                var withBlock = DataGridProcessStep;
                withBlock.DataSource = table;
                withBlock.DataBind();
                withBlock.SelectedIndex = -1;
                var loopTo = withBlock.Items.Count - 1;
                for (int i = 0; i <= loopTo; i++)
                {
                    // Get the group number.
                    item = withBlock.Items[i];
                    groupNumber = int.Parse(((Label)withBlock.Items[i].Cells[processStepGridColumns.GroupNumber.ToN()].FindControl("LabelGroupNumber")).Text);

                    entityID = int.Parse(((Label)withBlock.Items[i].Cells[processStepGridColumns.EntityID.ToN()].FindControl("LabelEntityID")).Text);

                    if (((Label)withBlock.Items[i].Cells[processStepGridColumns.ProcessActionID.ToN()].FindControl("LabelProcessActionID")).Text == string.Empty)
                        processActionID = 0;
                    else
                        processActionID = System.Convert.ToInt32(((Label)withBlock.Items[i].Cells[processStepGridColumns.ProcessActionID.ToN()].FindControl("LabelProcessActionID")).Text);

                    lbSelect = (LinkButton)withBlock.Items[i].Cells[processStepGridColumns.Select.ToN()].FindControl("LinkButtonSelect");
                    lbAction = (LinkButton)withBlock.Items[i].Cells[processStepGridColumns.Action.ToN()].FindControl("LinkButtonAction");

                    // First, enable the links and underline them.
                    lbSelect.Enabled = true;
                    lbSelect.Font.Underline = true;
                    lbAction.Enabled = true;
                    lbAction.Font.Underline = true;

                    // Check whether this row's Select and Action links should be hidden.
                    // Check that the action is one the allowed actions that reference a document upload.
                    switch (processActionID)
                    {
                        case var @case when @case == (int)eProcessAction.DocumentUploaded:
                        case var case1 when case1 == (int)eProcessAction.DocIssuedGenerated:
                        case var case2 when case2 == (int)eProcessAction.NoAction:
                            {
                                break;
                            }

                        default:
                            {
                                // Diable the links and don't underline them.
                                lbSelect.Enabled = false;
                                lbSelect.Font.Underline = false;
                                lbSelect.Visible = false;
                                lbAction.Enabled = false;
                                lbAction.Font.Underline = false;
                                break;
                            }
                    }

                    // Check whether the ClientID participates with the table of the current entity.
                    tableName = _entityTableList[entityID];
                    showLinkText = _tablePassList[tableName];

                    if (showLinkText == false)
                    {
                        lbSelect.Enabled = false;
                        lbSelect.Font.Underline = false;
                        lbSelect.Visible = false;
                        lbAction.Enabled = false;
                        lbAction.Font.Underline = false;
                    }

                    // Initially, set the item back color to unmarked (white).
                    item.BackColor = Color.FromArgb(Convert.ToInt32(UNMARKED_COLOR));

                    // If this group is a complete group (all Checked set to checked), set back color to completed (green).
                    if (groupNumber != prevGroupNumber)
                    {
                        isCompleteGroup = _dataComponent.IsCompleteGroup(groupNumber, processTypeID, clientCategoryID);
                        prevGroupNumber = groupNumber;
                    }

                    if (isCompleteGroup)
                        item.BackColor = Color.FromArgb(Convert.ToInt32(COMPLETED_GROUP_COLOR));

                    // If this is the current group number, set the back color to current (blue).
                    if (groupNumber == currentGroupNumber)
                        item.BackColor = Color.FromArgb(Convert.ToInt32(CURRENT_GROUP_COLOR));
                }
            }
        }

        /// <summary>
    /// Handles the DataGridProcessStep ItemCommand event. It loads the process step controls and shows the dialog.
    /// </summary>
    /// <param name="source">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DataGridProcessStep_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            int processStepID = System.Convert.ToInt32(DataGridProcessStep.DataKeys[e.Item.ItemIndex]);
            ViewState["ProcessStepID"] = processStepID;
            DataGridProcessStep.SelectedIndex = e.Item.ItemIndex;

            DataTable table = _dataComponent.GetProcessStep(processStepID);
            DataRow row = table.Rows[0];
            int groupNumber = row.ToInteger("GroupNumber");
            ViewState["CurrentGroupNumber"] = groupNumber;

            switch (e.CommandName)
            {
                case "Select":
                    {
                        LabelProcessTypeValue.Text = DropDownListProcessType.SelectedItem.Text;
                        LabelDescription.Text = row.ToString("Description");
                        CheckBoxChecked.Checked = row.ToBoolean("Checked");
                        LabelDateRecorded.Text = (row["DateRecorded"] == DBNull.Value) ? string.Empty : row.ToDateTime("DateRecorded").ToString("MMM d, yyyy");
                        TextBoxDocumentComments.Text = row.ToString("DocumentComments");
                        LabelDocumentPath.Text = row.ToString("DocumentPath");

                        // Disable the checkbox when it's an action step and the checkbox isn't checked. Don't want to check this step unless it passes its action.
                        string actionText = ((LinkButton)DataGridProcessStep.Items[e.Item.ItemIndex].Cells[processStepGridColumns.Action.ToN()].FindControl("LinkButtonAction")).Text.Trim();
                        if (actionText == string.Empty)
                            CheckBoxChecked.Enabled = true;
                        else if (CheckBoxChecked.Checked)
                            CheckBoxChecked.Enabled = true;
                        else
                            CheckBoxChecked.Enabled = false;

                        BindProcessStep();
                        OpenProcessStepDialog();
                        break;
                    }

                case "Action":
                    {
                        int processActionID = row.ToInteger("ProcessActionID");
                        int clientID = Convert.ToInt32(ViewState["ClientID"]);
                        int clientCategoryID = int.Parse(DropDownListService.SelectedItem.Value);
                        bool skipNextStepsIfPassed = row.ToBoolean("SkipNextStepsIfPassed");
                        int processTypeID = int.Parse(DropDownListProcessType.SelectedItem.Value);

                        ProcessAction(processStepID, processActionID, clientID, clientCategoryID, skipNextStepsIfPassed, groupNumber, processTypeID);
                        BindProcessStep();
                        break;
                    }
            }
        }

        /// <summary>
    /// Loads the two entity dictionaries.
    /// </summary>
        private void SetupDictionaries()
        {
            // Return if the dictionaries are already loaded.
            if (_entityTableList != null && _tablePassList != null)
                return;
            DataTable table;
            int entityID = 0;
            string tableName = string.Empty;

            _entityTableList = new Dictionary<int, string>();
            _tablePassList = new Dictionary<string, bool>();

            table = _dataComponent.GetEntities();
            foreach (DataRow row in table.Rows)
            {
                entityID = row.ToInteger("EntityID");
                tableName = row.ToString("TableName");

                if (entityID == 0)
                    continue;
                _entityTableList.Add(entityID, tableName);
            }

            DataSet ds = _dataComponent.GetEntityPass(_clientID);

            int staffCount = ds.Tables[(int)eTableNumber.lkpStaff].Rows.Count;
            int agencyCount = ds.Tables[(int)eTableNumber.tblAgency].Rows.Count;
            int clientCount = ds.Tables[(int)eTableNumber.tblClient].Rows.Count;
            int clientAssociationCount = ds.Tables[(int)eTableNumber.tblClientAssociation].Rows.Count;
            int clientJobCount = ds.Tables[(int)eTableNumber.tblClientJob].Rows.Count;
            int clientOccupantCount = ds.Tables[(int)eTableNumber.tblClientOccupant].Rows.Count;

            _tablePassList.Add("lkpStaff", staffCount > 0);
            _tablePassList.Add("tblAgency", agencyCount > 0);
            _tablePassList.Add("tblClient", clientCount > 0);
            _tablePassList.Add("tblClientAssociation", clientAssociationCount > 0);
            _tablePassList.Add("tblClientJob", clientJobCount > 0);
            _tablePassList.Add("tblClientOccupant", clientOccupantCount > 0);
        }

        /// <summary>
    /// Displays processStepDialog.
    /// </summary>
        private void OpenProcessStepDialog()
        {
            // Open the dialog.
            // Dim script As String = "<script language='javascript'>$(function() {{$('#processStepDialog').dialog('open');}});</script>"
            // Me.ClientScript.RegisterStartupScript(Me.GetType(), "Edit", script)
            // Me.ClientScript.RegisterStartupScript(Me.GetType(), "Edit", "<script>$('#processStepDialog').modal('show');</script>", False)
            this.ClientScript.RegisterStartupScript(this.GetType(), "Edit", "<script>$('#processStepDialog').modal('show');</script>");
        }

        /// <summary>
    /// Handles the ButtonSave Click event. It saves data to tblProcessStep.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonSave_Click(object sender, System.EventArgs e)
        {
            int processStepID = System.Convert.ToInt32(ViewState["ProcessStepID"]);
            var documentPath = LabelDocumentPath.Text.Trim();
            string documentComments = TextBoxDocumentComments.Text.Trim();

            _dataComponent.UpdateProcessStep(processStepID, documentPath, documentComments, CheckBoxChecked.Checked, Session["UserEmailAddress"].ToString());
            BindProcessStep();
        }



        /// <summary>
    /// This routine handles the ButtonUploadDocument Click event. It uploads the requested file, then saves
    /// the filename to tblProcessStep.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonUploadDocument_Click(object sender, System.EventArgs e)
        {
            try
            {
                // Use the FileUploadDocumentation control...
                {
                    var withBlock = FileUploadDocuments;
                    if (withBlock.HasFile)
                    {
                        int processStepID = System.Convert.ToInt32(ViewState["ProcessStepID"]);

                        // Create the documentPath in the form:
                        // <file name> (<type><ID>).<ext>.
                        int loc = withBlock.FileName.LastIndexOf('.');
                        string filePart = default(string);
                        string ext = default(string);
                        string type = "CS"; // Cs means client services.

                        if (loc >= 0)
                        {
                            filePart = withBlock.FileName.Substring(0, loc);
                            ext = withBlock.FileName.Substring(loc);
                        }
                        else
                        {
                            filePart = withBlock.FileName;
                            ext = string.Empty;
                        }

                        if (ext.ToLower() != PDF_FILE)
                        {
                            ShowMessage(LabelErrorStep, "Only PDF file types can be uploaded.");
                            return;
                        }

                        string documentPath = string.Format("{0} ({1}{2}){3}", filePart, type, processStepID, ext);

                        // Define the AgencyInfo folder.
                        string documentFolder = Utility.documentsFolder;

                        // Prepend the upload folder to the documentPath.
                        string savePath = Server.MapPath(documentFolder) + documentPath;

                        // If the file already exists, delete it; then save the uploaded file to the save path.
                        if (File.Exists(savePath))
                            File.Delete(savePath);
                        withBlock.SaveAs(savePath);

                        // Save the documentPath, documentation comments, email, and set the date time uploaded.
                        string documentComments = TextBoxDocumentComments.Text.Trim();
                        _dataComponent.UpdateProcessStep(processStepID, documentPath, documentComments, CheckBoxChecked.Checked, Session["UserEmailAddress"].ToString());

                        // Set controls to acknowledge a successful upload.
                        ShowMessage(LabelErrorStep, "File successfully uploaded.", true);

                        LabelDateRecorded.Text = DateTime.Now.Date.ToString("MMM d, yyyy");
                        LabelDocumentPath.Text = documentPath;
                        BindProcessStep();
                    }
                    else
                        // If no file was entered, prompt the user to enter a file name.
                        ShowMessage(LabelErrorStep, "Please select a file to upload.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage(LabelErrorStep, "An upload error has occurred.");
            }

            finally
            {
                OpenProcessStepDialog();
            }
        }



        /// <summary>
    /// Processes the action. Sets tblProcessStep.Checked, processes pass/fail and sets checked on this and subsequent steps if skip set, reloads the grid,
    /// shows message, error or advisory.
    /// </summary>
    /// <param name="processStepID">Process Step ID.</param>
    /// <param name="processActionID">Process Action ID.</param>
    /// <param name="clientID">Client ID.</param>
    /// <param name="clientCategoryID">Client Category ID.</param>
    /// <param name="skipNextStepsIfPassed">Skip Next Steps If Passed.</param>
    /// <param name="groupNumber">Group Number.</param>
    /// <param name="processTypeID">Process Type ID.</param>
        private void ProcessAction(int processStepID, int processActionID, int clientID, int clientCategoryID, bool skipNextStepsIfPassed, int groupNumber, int processTypeID)
        {
            bool passed = false;
            bool found = false;
            int categoryID = _dataComponent.GetCategoryID(clientCategoryID);
            DataTable table = null;
            string sql = string.Empty;
            DataRow row = null;

            switch (processActionID)
            {
                case var @case when @case == (int)eProcessAction.ClientQualified:
                    {
                        passed = ClientQualified(processStepID, processActionID, clientID, categoryID, clientCategoryID, skipNextStepsIfPassed, groupNumber, processTypeID);
                        break;
                    }

                case var case1 when case1 == (int)eProcessAction.RegisteredForService:
                    {
                        sql = string.Format(GmWeb.Logic.Resource.RegisteredForServiceSQL, clientCategoryID);
                        table = _dataComponent.GetGeneric(sql);
                        if (table.Rows.Count > 0)
                        {
                            ShowMessage(LabelError, "Action succeeded", true);
                            passed = true;
                        }
                        else
                        {
                            ShowMessage(LabelError, "Action 'Registered for service' failed.");
                            passed = false;
                        }

                        break;
                    }

                case var case2 when case2 == (int)eProcessAction.DocumentUploaded:
                    {
                        table = _dataComponent.GetProcessStep(processStepID);
                        row = table.Rows[0];
                        if (row["DateDocumentLoaded"] == DBNull.Value)
                        {
                            ShowMessage(LabelError, "Action 'Document uploaded' failed. Please click the Select link and upload the requested document. Then try again.");
                            passed = false;
                        }
                        else
                        {
                            ShowMessage(LabelError, "Action succeeded", true);
                            passed = true;
                        }

                        break;
                    }

                case var case3 when case3 == (int)eProcessAction.DocIssuedGenerated:
                    {
                        passed = _dataComponent.CheckDocIssuedGenerated(processStepID, processTypeID, clientCategoryID, Convert.ToInt32(Session["AgencyIDParent"]));

                        if (passed)
                            ShowMessage(LabelError, "Action succeeded", true);
                        else
                            ShowMessage(LabelError, "Action 'Doc issued/generated' failed. Please click the Select link and upload the requested document. Then try again.");
                        break;
                    }

                case var case4 when case4 == (int)eProcessAction.IncompleteStep:
                    {
                        sql = string.Format(GmWeb.Logic.Resource.IncompleteStepSQL, clientCategoryID, processTypeID);
                        table = _dataComponent.GetGeneric(sql);
                        if (table.Rows.Count > 0)
                        {
                            ShowMessage(LabelError, "Action succeeded", true);
                            passed = true;
                        }
                        else
                        {
                            ShowMessage(LabelError, "Action 'Incomplete step' failed.");
                            passed = false;
                        }

                        break;
                    }

                case var case5 when case5 == (int)eProcessAction.ServicePaymentMade:
                    {
                        passed = _dataComponent.FeePaid(clientCategoryID);

                        if (passed)
                        {
                            ShowMessage(LabelError, "Action succeeded", true);
                            passed = true;
                        }
                        else
                        {
                            ShowMessage(LabelError, "Action 'Service payment made' failed.");
                            passed = false;
                        }

                        break;
                    }

                case var case6 when case6 == (int)eProcessAction.AttendedService:
                    {
                        sql = string.Format(GmWeb.Logic.Resource.AttendedServiceSQL, clientCategoryID);
                        table = _dataComponent.GetGeneric(sql);
                        if (table.Rows.Count > 0)
                        {
                            ShowMessage(LabelError, "Action succeeded", true);
                            passed = true;
                        }
                        else
                        {
                            ShowMessage(LabelError, "Action 'Attended service' failed.");
                            passed = false;
                        }

                        break;
                    }

                case var case7 when case7 == (int)eProcessAction.ServiceSigRequired:
                    {
                        sql = string.Format(GmWeb.Logic.Resource.ServiceSigRequiredSQL, clientCategoryID);
                        table = _dataComponent.GetGeneric(sql);
                        if (table.Rows.Count > 0)
                        {
                            ShowMessage(LabelError, "Action succeeded", true);
                            passed = true;
                        }
                        else
                        {
                            ShowMessage(LabelError, "Action 'Service sig required' failed.");
                            passed = false;
                        }

                        break;
                    }

                case var case8 when case8 == (int)eProcessAction.Test:
                    {
                        passed = true;
                        break;
                    }

                default:
                    {
                        ShowMessage(LabelError, "This action has not yet been implemented.");
                        passed = false;
                        return;
                    }
            }

            if (passed)
            {
                table = _dataComponent.GetProcessSteps(processTypeID, clientCategoryID, Convert.ToInt32(Session["AgencyIDParent"]));
                table.DefaultView.RowFilter = string.Format("GroupNumber={0}", groupNumber);

                foreach (System.Data.DataRowView rv in table.DefaultView)
                {
                    if (found)
                        _dataComponent.UpdateProcessStepChecked(rv.ToInteger("ProcessStepID"), Session["UserEmailAddress"].ToString());
                    else if (rv.ToInteger("ProcessStepID") == processStepID)
                    {
                        found = true;
                        _dataComponent.UpdateProcessStepChecked(processStepID, Session["UserEmailAddress"].ToString());

                        if (skipNextStepsIfPassed == false)
                            break;
                    }
                }
            }
        }



        private bool ClientQualified(int processStepID, int processActionID, int clientID, int categoryID, int clientCategoryID, bool skipNextStepsIfPassed, int groupNumber, int processTypeID)
        {
            StringBuilder sb = new StringBuilder();

            DataTable tableCategory = _dataComponent.GetProfiles("CAT", categoryID);
            DataTable tableClient = _dataComponent.GetProfiles("CLI", clientID);
            double threshold = 75.0D;
            double totalCount = 0.0D;
            double matchCount = 0.0D;

            if (tableCategory.Rows.Count == 0)
                sb.AddMessage("Cannot link because the category profile hasn't been created.");
            if (tableClient.Rows.Count == 0)
                sb.AddMessage("Cannot link because the resident (client) profile hasn't been created.");

            if (sb.Length > 0)
            {
                ShowMessage(LabelError, sb.ToString());
                return false;
            }

            string name = default(string);
            string value = default(string);
            string valueClientString = default(string);
            string dataType = default(string);
            DataView viewClient = new DataView(tableClient);

            foreach (DataRow rowCategory in tableCategory.Rows)
            {
                name = rowCategory.ToString("Name");
                value = rowCategory.ToString("Value");
                dataType = rowCategory.ToString("DataType");
                viewClient.RowFilter = string.Format("Name='{0}' AND DataType='{1}'", name, dataType);

                if (viewClient.Count > 0)
                {
                    totalCount += 1.0D;
                    valueClientString = viewClient[0].ToString("Value");
                    switch (dataType)
                    {
                        case "B":
                        case "L":
                        case "S":
                            {
                                
                                if (valueClientString == value)
                                    matchCount += 1.0D;
                                else
                                    sb.AddMessage(string.Format("Category profile '{0}' does not match the client profile value", name));
                                break;
                            }

                        case "N":
                            {
                                if (int.Parse(valueClientString) <= int.Parse(value))
                                    matchCount += 1.0D;
                                else
                                    sb.AddMessage(string.Format("Category profile '{0}' does not match the client profile value", name));
                                break;
                            }

                        case "D":
                            {
                                if (DateTime.Parse(valueClientString) == DateTime.Parse(value))
                                    matchCount += 1.0D;
                                else
                                    sb.AddMessage(string.Format("Category profile '{0}' does not match the client profile value", name));
                                break;
                            }
                    }
                }
            }

            bool isMatch = (totalCount > 0.0D) ? ((100.0D * (matchCount / totalCount)) >= threshold) : false;
            if (isMatch || sb.Length == 0)
            {
                _dataComponent.LogProfileMatch(clientID, categoryID, "Profiles match.", Session["UserEmailAddress"].ToString());
                return true;
            }
            else
            {
                ShowMessage(LabelError, sb.ToString());
                _dataComponent.LogProfileMatch(clientID, categoryID, sb.ToString().Replace("<br />", global::Utility.Newline), Session["UserEmailAddress"].ToString());
                return false;
            }
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
