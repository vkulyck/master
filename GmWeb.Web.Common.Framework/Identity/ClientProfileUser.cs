using System;
using Microsoft.AspNet.Identity;
using GmWeb.Logic.Interfaces;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Web.Common.Identity
{
    // You can add profile data for the user by adding more properties to your AppIdentity class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    [NotMapped]
    public class ClientProfileUser : GmIdentity, IMultiQueryDataRowModel
    {
        [NotMapped]
        public string AgencyName { get; set; }
        public int ParentAgencyID { get; set; }
        public int DefaultClientDataTypeID { get; set; } = 801;

        public void CopyRowData(DataRow row)
        {
            this.ClientID = row.ToInteger("ClientID");
            this.AgencyName = row.ToString("AgencyName");
            this.FirstName = row.ToString("FirstName");
            this.LastName = row.ToString("LastName");
            this.Email = row.ToString("Email");
            this.AgencyID = row.ToInteger("AgencyID");
            this.PasswordHash = row.ToString("PasswordHash");
        }

        public void PerformAdditionalQueries(DataComponent dc)
        {
            if(this.AgencyID.HasValue)
                this.ParentAgencyID = dc.GetParentAgencyID(this.AgencyID.Value);
        }
    }
}
