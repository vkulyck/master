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
    partial class Skillset : System.Web.UI.Page
    {
        // Empty ASP controls to fix immediate build issues
        Label LabelLanguages, LabelError, LabelSkillDetail, LabelAddSkillset;
        TextBox TextBoxLanguages;
        DropDownList DropDownListSkillSubCategory, DropDownListSkillCategory, DropDownListSkillDetail;
        DataGrid DataGridSkillset;
        //********************************************//

        private DataComponent _dataComponent;

        private enum gridColumnsSkillset
        {
            Select,
            Delete,
            SkillCategory,
            SkillSubCategory,
            SkillDetail,
            Languages
        }

        private const string ADD_MESSAGE = "Add a skill not listed above:";
        private const string EDIT_MESSAGE = "Edit the selected skill:";



        /// <summary>
    /// Handles the Page Load event. It instantiates a DataComponent object, then sets controls on the page.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            _dataComponent = new DataComponent();
            LabelError.Visible = false;

            if (DropDownListSkillSubCategory.Items.Count > 0 && DropDownListSkillSubCategory.SelectedItem.Text.StartsWith("Language"))
            {
                LabelLanguages.Visible = true;
                TextBoxLanguages.Visible = true;
            }
            else
            {
                LabelLanguages.Visible = false;
                TextBoxLanguages.Visible = false;
            }

            if (!IsPostBack)
            {
                LoadSkillset();
                LoadDDLs();
            }
        }

        /// <summary>
    /// Loads DataGridSkillset
    /// </summary>
        private void LoadSkillset()
        {
            int clientID = System.Convert.ToInt32(Session["ClientID"]);

            {
                var withBlock = DataGridSkillset;
                withBlock.DataSource = _dataComponent.GetSkillSet(clientID);
                withBlock.DataBind();
            }
        }

        /// <summary>
    /// Loads the DDLs on this screen.
    /// </summary>
        private void LoadDDLs()
        {
            {
                var withBlock = DropDownListSkillCategory;
                withBlock.DataSource = _dataComponent.GetSkillCategories();
                withBlock.DataBind();
            }

            {
                var withBlock1 = DropDownListSkillSubCategory;
                withBlock1.DataSource = null;
                withBlock1.DataBind();
            }

            {
                var withBlock2 = DropDownListSkillDetail;
                withBlock2.DataSource = null;
                withBlock2.DataBind();
            }
        }

        /// <summary>
    /// Handles the DropDownListSkillCategory SelectedIndexChanged event. It loads DropDownListSkillSubCategory and empties DropDownListSkillDetail.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DropDownListSkillCategory_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (DropDownListSkillCategory.SelectedIndex <= 0)
                return;

            int skillCategoryID = int.Parse(DropDownListSkillCategory.SelectedItem.Value);

            {
                var withBlock = DropDownListSkillSubCategory;
                withBlock.DataSource = _dataComponent.GetSkillSubCategories(skillCategoryID);
                withBlock.DataBind();
            }

            {
                var withBlock1 = DropDownListSkillDetail;
                withBlock1.DataSource = null;
                withBlock1.DataBind();
            }

            LabelLanguages.Visible = false;
            TextBoxLanguages.Visible = false;
        }

        /// <summary>
    /// Handles the DropDownListSkillSubCategory SelectedIndexChanged event. It loads DropDownListSkillDetail.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DropDownListSkillSubCategory_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (DropDownListSkillSubCategory.SelectedIndex <= 0)
                return;

            int skillSubCategoryID = int.Parse(DropDownListSkillSubCategory.SelectedItem.Value);
            DataTable table = _dataComponent.GetSkillDetails(skillSubCategoryID);

            if (table.Rows.Count > 1)
            {
                {
                    var withBlock = DropDownListSkillDetail;
                    withBlock.DataSource = table;
                    withBlock.DataBind();
                }

                LabelSkillDetail.Visible = true;
                DropDownListSkillDetail.Visible = true;
            }
            else
            {
                LabelSkillDetail.Visible = false;
                DropDownListSkillDetail.Visible = false;
            }
        }

        /// <summary>
    /// Handles the DataGridSkillset ItemCommand event. It processes the Select and Delete commands.
    /// </summary>
    /// <param name="source">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void DataGridSkillset_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Select":
                    {
                        DataGridSkillset.SelectedIndex = e.Item.ItemIndex;

                        DataGridItem item = DataGridSkillset.Items[e.Item.ItemIndex];
                        string category = ((Label)item.Cells[(int)gridColumnsSkillset.SkillCategory].FindControl("LabelSkillsetCategory")).Text;
                        string subCategory = ((Label)item.Cells[(int)gridColumnsSkillset.SkillSubCategory].FindControl("LabelSkillsetSubCategory")).Text;
                        string detail = ((Label)item.Cells[(int)gridColumnsSkillset.SkillDetail].FindControl("LabelSkillDetail")).Text;
                        string languages = ((Label)item.Cells[(int)gridColumnsSkillset.Languages].FindControl("LabelSkillDescription")).Text;

                        LoadDDLs();
                        DropDownListSkillCategory.SetText(category);
                        DropDownListSkillCategory_SelectedIndexChanged(this, new EventArgs());

                        DropDownListSkillSubCategory.SetText(subCategory);
                        DropDownListSkillSubCategory_SelectedIndexChanged(this, new EventArgs());

                        DropDownListSkillDetail.SetText(detail);
                        TextBoxLanguages.Text = languages;

                        LabelAddSkillset.Text = EDIT_MESSAGE;

                        if (detail == string.Empty)
                        {
                            LabelSkillDetail.Visible = false;
                            DropDownListSkillDetail.Visible = false;
                        }
                        else
                        {
                            LabelSkillDetail.Visible = true;
                            DropDownListSkillDetail.Visible = true;
                        }

                        if (languages == string.Empty)
                        {
                            LabelLanguages.Visible = false;
                            TextBoxLanguages.Visible = false;
                        }
                        else
                        {
                            LabelLanguages.Visible = true;
                            TextBoxLanguages.Visible = true;
                        }

                        break;
                    }

                case "Delete":
                    {
                        int skillSetID = System.Convert.ToInt32(DataGridSkillset.DataKeys[e.Item.ItemIndex]);
                        _dataComponent.DeleteSkillSet(skillSetID);

                        LabelAddSkillset.Text = ADD_MESSAGE;
                        DataGridSkillset.SelectedIndex = -1;
                        LoadSkillset();
                        break;
                    }
            }
        }

        /// <summary>
    /// Handles the ButtonSubmit Click event. It saves the skill set data and reloads the screen's controls.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonSubmit_Click(object sender, System.EventArgs e)
        {
            if (IsValidData() == false)
                return;

            int clientID = System.Convert.ToInt32(Session["ClientID"]);
            int skillCategoryID = int.Parse(DropDownListSkillCategory.SelectedItem.Value);
            int skillSubCategoryID = int.Parse(DropDownListSkillSubCategory.SelectedItem.Value);
            int skillDetailID = (DropDownListSkillDetail.Items.Count > 0) ? int.Parse(DropDownListSkillDetail.SelectedItem.Value) : 0;
            string skillDescription = TextBoxLanguages.Visible ? TextBoxLanguages.Text.Trim() : string.Empty;

            if (LabelAddSkillset.Text.StartsWith("Add"))
                _dataComponent.SaveSkillset(clientID, skillCategoryID, skillSubCategoryID, skillDetailID, skillDescription);
            else
            {
                int skillSetID = System.Convert.ToInt32(DataGridSkillset.DataKeys[DataGridSkillset.SelectedIndex]);
                _dataComponent.UpdateSkillset(skillSetID, skillCategoryID, skillSubCategoryID, skillDetailID, skillDescription);
            }

            ShowMessage("Data saved.", true);
            ButtonReset_Click(this, new EventArgs());
        }

        /// <summary>
    /// Resets the skillset controls.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void ButtonReset_Click(object sender, System.EventArgs e)
        {
            LoadSkillset();
            LoadDDLs();

            DropDownListSkillSubCategory.Items.Clear();

            LabelSkillDetail.Visible = false;
            {
                var withBlock = DropDownListSkillDetail;
                withBlock.Visible = false;
                withBlock.Items.Clear();
            }

            LabelLanguages.Visible = false;
            TextBoxLanguages.Visible = false;

            LabelAddSkillset.Text = ADD_MESSAGE;
            DataGridSkillset.SelectedIndex = -1;
        }

        /// <summary>
    /// Checks if the screen data is valid.
    /// </summary>
    /// <returns>True if valid; else False.</returns>
        private bool IsValidData()
        {
            StringBuilder sb = new StringBuilder();

            if (DropDownListSkillCategory.SelectedIndex <= 0)
                sb.AddMessage("Please select a skill category.");
            else if (DropDownListSkillSubCategory.SelectedIndex <= 0)
                sb.AddMessage("Please select a sub-category.");
            else if (DropDownListSkillDetail.Items.Count > 1 && DropDownListSkillDetail.SelectedIndex <= 0)
                sb.AddMessage("Please select a detail.");

            if (TextBoxLanguages.Visible && TextBoxLanguages.Text.Trim() == string.Empty)
                sb.AddMessage("Please enter one or more languages.");

            if (sb.Length == 0)
                return true;
            else
            {
                ShowMessage(sb.ToString());
                return false;
            }
        }

        /// <summary>
    /// Handles the LinkButtonHome Click event. It routes control to the Home screen.
    /// </summary>
    /// <param name="sender">The object which initiated the event.</param>
    /// <param name="e">The arguments associated with the event.</param>
        protected void LinkButtonHome_Click(object sender, System.EventArgs e)
        {
            Server.Transfer("Home.aspx");
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
    }
}
