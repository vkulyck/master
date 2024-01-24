using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Demographics
{
    [Table("tblLivingWageEstimates", Schema = "dmg")]
    public class LivingWageEstimate : BaseDataModel
    {
        [Key]
        public int LivingWageEstimateID { get; set; }
        public string StateCode { get; set; }
        public string CountyCode { get; set; }
        [SqlDataType(System.Data.SqlDbType.Int)]
        public EarnerType EarnerType { get; set; }
        public int NumberOfChildren { get; set; }
        [SqlDataType(System.Data.SqlDbType.Int)]
        public WageThresholdType WageThresholdType { get; set; }
        public decimal WageEstimate { get; set; }

        public override string ToString() =>
            $"{this.LivingWageEstimateID}: {this.CountyCode}|{this.StateCode}"
            + $"{this.EarnerType}, {this.WageThresholdType}, {this.NumberOfChildren} Kids,"
            + $"{this.WageEstimate:C2}"
        ;
    }
}
