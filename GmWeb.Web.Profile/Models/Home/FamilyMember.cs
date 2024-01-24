using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Profile.Models.Home
{
    public class FamilyMember : IDataRowModel
    {
        public string AddressNo { get; set; }
        public string AddressStreet { get; set; }
        public string AddressStreetType { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public void CopyRowData(DataRow row) => this.PerformDefaultRowConversion(row);
    }
}