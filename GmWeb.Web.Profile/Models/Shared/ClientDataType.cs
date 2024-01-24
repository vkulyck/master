using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace GmWeb.Web.Profile.Models.Shared
{
    public class ClientDataType : BaseEntityViewModel
    {
        public string Description { get; set; }
        public string Category { get; set; }
        public int ClientDataTypeID { get; set; }
        public int? NextClientDataTypeID { get; set; }
        public int? PrevClientDataTypeID { get; set; }

        public override void CopyRowData(DataRow row) 
        {
            this.ClientDataTypeID = row.ToInteger("ClientDataTypeID");
            this.Description = row.ToString("Description", AllowMissing: true);
            this.Category = row.ToString("Category");
            this.NextClientDataTypeID = row.ToNullableInteger("NextClientDataTypeID", AllowMissing: true);
            this.PrevClientDataTypeID = row.ToNullableInteger("PrevClientDataTypeID", AllowMissing: true);
        }
    }
}