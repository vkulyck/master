using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Profile.Models.Home
{
    public class ClientService : IDataRowModel
    {
        public string Signature { get; set; }
        public int CategoryID { get; set; }
        public int ClientCategoryID { get; set; }
        public int ClientCategoryDateID { get; set; }
        public string Phone { get; set; }
        public string AgencyName { get; set; }
        public bool SignatureRequied { get; set; }
        public int? AgencyConsentID { get; set; }
        public string ServiceName { get; set; }
        public string Venue { get; set; }
        public DateTime ScheduledDate { get; set; }
        public void CopyRowData(DataRow row) => this.PerformDefaultRowConversion(row);
    }
}