using System;
using System.Runtime.CompilerServices;
using System.Data;
using System.Web.UI.WebControls;
using GmWeb.Logic.Utility.Extensions;

/// <summary>

/// This module contains extension methods on the DataRow and DataRowView objects.

/// </summary>
public static class ControlExtensions
{
    /// <summary>
    /// Sets a DropDownList's SelectedIndex to 0.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    public static void Reset(this DropDownList ddl)
    {
        if (ddl.Items.Count > 0)
            ddl.SelectedIndex = 0;
    }

    /// <summary>
    /// Selects a DropDownLists item, given a DataRol and column name.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    /// <param name="row">The DataRow containing the value to set.</param>
    /// <param name="column">The row's column containing the data to set.</param>
    public static void SetValue(this DropDownList ddl, DataRow row, string column)
    {
        try
        {
            ddl.SelectedValue = row.ToString(column).Trim();
        }
        catch
        {
            if (ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Selects a DropDownLists item, given a DataRol and column name.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    /// <param name="value">The DropDownList value to select.</param>
    public static void SetValue(this DropDownList ddl, string value)
    {
        try
        {
            value = value.Trim();
            var loopTo = ddl.Items.Count - 1;
            for(int Index = 0; Index <= loopTo; Index++)
            {
                if (value == ddl.Items[Index].Value.Trim())
                {
                    ddl.SelectedIndex = Index;
                    return;
                }
            }
        }
        catch
        {
            if (ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Selects a DropDownLists item, given a DataRol and column name.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    /// <param name="value">The DropDownList value to select.</param>
    public static void SetValue(this DropDownList ddl, DateTime value)
    {
        try
        {
            var loopTo = ddl.Items.Count - 1;
            // ddl.SelectedValue = value.Trim()
            for(int Index = 0; Index <= loopTo; Index++)
            {
                if (value == DateTime.Parse(ddl.Items[Index].Value))
                {
                    ddl.SelectedIndex = Index;
                    break;
                }
            }
        }
        catch
        {
            if (ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Binds a DropDownList to a DataTable; sets the SelectedIndex to 0 if there's any data loaded.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    /// <param name="table">The DataTable with the data being bound.</param>
    public static void Bind(this DropDownList ddl, DataTable table)
    {
        {
            var withBlock = ddl;
            withBlock.DataSource = table;
            withBlock.DataBind();
            if (withBlock.Items.Count > 0)
                withBlock.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Binds a DropDownList to Nothing.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    public static void Bind(this DropDownList ddl)
    {
        {
            var withBlock = ddl;
            withBlock.DataSource = null;
            withBlock.DataBind();
        }
    }

    /// <summary>
    /// Returns a DropDownList's SelectedValue as an integer; when there's no selected value, it returns -1.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    /// <returns>The DropDownList's SelectedValue; when there's no selected value, it returns -1.</returns>
    public static int Value(this DropDownList ddl)
    {
        return (ddl.SelectedIndex > 0) ? System.Convert.ToInt32(ddl.SelectedValue) : -1;
    }

    /// <summary>
    /// Returns a DropDownList's SelectedValue as string; when there's no selected value, it returns an empty string.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    /// <returns>The DropDownList's SelectedValue; when there's no selected value, it returns an empty string.</returns>
    public static string ValueString(this DropDownList ddl)
    {
        return (ddl.SelectedIndex > 0) ? ddl.SelectedValue : string.Empty;
    }

    /// <summary>
    /// Returns a DropDownList's SelectedValue as a date; when there's no selected value, it returns -1.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    /// <returns>The DropDownList's SelectedValue; when there's no selected value, it returns -1.</returns>
    public static DateTime ValueDate(this DropDownList ddl)
    {
        return (ddl.SelectedIndex > 0) ? Convert.ToDateTime(ddl.SelectedValue) : Utility.NullDate;
    }

    /// <summary>
    /// Sets a DropDownLists index, given a value.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    /// <param name="value">The DropDownList value to select.</param>
    public static void SetIndex(this DropDownList ddl, string value)
    {
        try
        {
            var loopTo = ddl.Items.Count - 1;
            for(int Index = 0; Index <= loopTo; Index++)
            {
                if (ddl.Items[Index].Value == value)
                {
                    ddl.SelectedIndex = Index;
                    break;
                }
            }
        }
        catch
        {
            if (ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Sets a DropDownLists index, given a value.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    /// <param name="value">The DropDownList value to select.</param>
    public static void SetIndex(this DropDownList ddl, int value)
    {
        try
        {
            var loopTo = ddl.Items.Count - 1;
            for(int Index = 0; Index <= loopTo; Index++)
            {
                if (ddl.Items[Index].Value == value.ToString())
                {
                    ddl.SelectedIndex = Index;
                    break;
                }
            }
        }
        catch
        {
            if (ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Sets a DropDownLists index, given its text.
    /// </summary>
    /// <param name="ddl">The DropDownList.</param>
    /// <param name="value">The DropDownList text to select.</param>
    public static void SetText(this DropDownList ddl, string value)
    {
        try
        {
            value = value.Trim();
            var loopTo = ddl.Items.Count - 1;
            for(int Index = 0; Index <= loopTo; Index++)
            {
                if (ddl.Items[Index].Text.Trim() == value)
                {
                    ddl.SelectedIndex = Index;
                    break;
                }
            }
        }
        catch
        {
            if (ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Removes a TextBox number's commas ( , ) and dollar signs ( $ ).
    /// </summary>
    /// <param name="tb">The TextBox to be examined.</param>
    /// <returns>The TextBox.Text stripped of commas and dollar signs.</returns>
    public static string ValueStripped(this TextBox tb)
    {
        return tb.Text.Trim().Replace("$", "").Replace(",", "");
    }

    /// <summary>
    /// Removes a TextBox number's commas ( , ) and dollar signs ( $ ) and converts the value to an integer.
    /// If TextBox.Text is empty, it returns 0.
    /// </summary>
    /// <param name="tb">The TextBox to be examined.</param>
    /// <returns>The TextBox.Text stripped of commas and dollar signs and converted to an integer.</returns>
    public static int ToInteger(this TextBox tb)
    {
        string text = tb.Text.Trim();

        if (text == string.Empty)
            return 0;
        else
            return int.Parse(tb.Text.Trim().Replace("$", "").Replace(",", ""));
    }

    /// <summary>
    /// Removes a TextBox number's commas ( , ) and dollar signs ( $ ) and converts the value to a decimal.
    /// If TextBox.Text is empty, it returns 0.
    /// </summary>
    /// <param name="tb">The TextBox to be examined.</param>
    /// <returns>The TextBox.Text stripped of commas and dollar signs and converted to a decimal.</returns>
    public static decimal ToDecimal(this TextBox tb)
    {
        string text = tb.Text.Trim();

        if (text == string.Empty)
            return 0M;
        else
            return decimal.Parse(tb.Text.Trim().Replace("$", "").Replace(",", ""));
    }

    /// <summary>
    /// Converts TextBox.Text to a formatted phone number. If TextBox.Text is empty, it returns an empty string.
    /// </summary>
    /// <param name="tb">The TextBox to be examined.</param>
    /// <returns>TextBox.Text as a formatted phone number.</returns>
    public static string ToPhone(this TextBox tb)
    {
        string text = tb.Text.Trim();

        if (text == string.Empty)
            return text;
        else
        {
            string value;
            string field = Utility.RemoveCharacters(text, " ()-");

            if (string.IsNullOrEmpty(field))
                return "";

            long fieldLong = long.Parse(field);

            switch (field.Length)
            {
                case 7:
                    {
                        value = fieldLong.ToString("000-0000");
                        break;
                    }

                case 10:
                    {
                        value = fieldLong.ToString("000-000-0000");
                        break;
                    }

                default:
                    {
                        value = field;
                        break;
                    }
            }

            return value;
        }
    }
}

