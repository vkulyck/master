using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GmWeb.Logic.Data.Annotations;

namespace GmWeb.Web.Demographics.Logic.DataModels
{
    public class CityIncomeLevel
    {
        public string StateCode { get; set; }
        public string CountyCode { get; set; }
        [SqlDataType(System.Data.SqlDbType.Int)]
        public IncomeLevel IncomeLevel { get; set; }
        public int HouseholdCount { get; set; }
    }
}
