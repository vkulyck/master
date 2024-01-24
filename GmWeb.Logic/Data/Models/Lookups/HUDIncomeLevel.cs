using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Lookups
{
    [Table("lkpHUDIncomeLevel")]
    public class HUDIncomeLevel : BaseDataModel
    {
        [Key]
        public int HUDIncomeLevelID { get; set; }
        public int Year { get; set; }
        public string StateCode { get; set; }
        public string CountyCode { get; set; }
        [SqlDataType(System.Data.SqlDbType.Int)]
        public IncomeLevel IncomeLevel { get; set; }
        public int TotalFamilyIncome { get; set; }
        public int NumberInFamily { get; set; }
    }
}