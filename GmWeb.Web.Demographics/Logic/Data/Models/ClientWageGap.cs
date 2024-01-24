using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Data.Annotations;

namespace GmWeb.Web.Demographics.Logic.DataModels
{
    public class ClientWageGap
    {
        [SqlDataType(System.Data.SqlDbType.Int)]
        public IncomeLevel IncomeLevel { get; set; }
        public decimal WageGap { get; set; }
        public int HouseholdCount { get; set; }
        public string ActivityID { get; set; }
    }
}