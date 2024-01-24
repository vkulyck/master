using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Profile.Models.ProfileSearch
{
    public class ProfileRow : IMultiQueryDataRowModel, IDynamicValueType
    {
        public int UniqueID { get; set; }
        public bool IsChecked { get; set; }
        public string ProfileName { get; set; }
        
        [ScriptIgnore]
        public int? LookupID { get; set; }
        [ScriptIgnore]
        public string LookupTable { get; set; }

        #region Dynamic Value Properties
        public string DataType { get; set; }
        public string Value { get; set; }
        public int? IntValue => this.GetIntValue();
        public bool? BoolValue => this.GetBoolValue();
        public int? ListReferenceValue => this.GetListReferenceValue();
        public DateTime? DateValue => this.GetDateValue();
        #endregion

        public void PerformAdditionalQueries(DataComponent dc)
        {
            if (this.DataType != "L")
                return;
            var table = dc.GetProfileDDLData(this.LookupTable);
            foreach(DataRow row in table.Rows)
            {
                if (row.ToInteger("ID") == this.LookupID)
                    this.Value = row.ToString("Description");
            }
        }

        public void CopyRowData(DataRow row)
        {
            this.UniqueID = row.ToInteger("ClientServiceProfileID");
            this.ProfileName = row.ToString("Name");
            this.LookupTable = row.ToString("LookupTableName");
            this.CopyDynamicRowData(row);
        }
    }
}