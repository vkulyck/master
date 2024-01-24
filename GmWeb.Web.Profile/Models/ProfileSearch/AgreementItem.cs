using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Interfaces;

namespace GmWeb.Web.Profile.Models.ProfileSearch
{
    public class AgreementItem : IDataRowModel
    {
        public int AgencyConsentID { get; set; }
        public string Description { get; set; }
        public string LegalVerbiage { get; set; }

        public void CopyRowData(DataRow row) => this.PerformDefaultRowConversion(row);
    }
}