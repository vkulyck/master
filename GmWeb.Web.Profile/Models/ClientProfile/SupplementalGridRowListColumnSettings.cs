using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using GmWeb.Web.Common.Utility;
using System.Web.Mvc;

namespace GmWeb.Web.Profile.Models.ClientProfile
{
    public class SupplementalGridRowListColumnSettings
    {
        public string ProfileName { get; set; }
        public string LookupTableName { get; set; }
        public string DefaultSortColumn
        {
            get
            {
                if (this.LookupTableName == "lkpEducationLevel")
                    return "ID";
                return "Description";
            }
        }
        public string OptionalRowText
        {
            get
            {
                switch(this.ProfileName)
                {
                    case "Head of Household": return "(None)";
                    case "OccupationType": return "Not Applicable";
                    default: return "(Select)";
                }
            }
        }

        public List<SelectListItem> Items { get; set; }

        public void SetItems(string dataType, DataTable table)
        {
            if (dataType != "L")
                return;
            this.Items = table.ToListItems();
        }
    }
}