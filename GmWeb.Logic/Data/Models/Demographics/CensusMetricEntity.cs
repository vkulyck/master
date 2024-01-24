using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Demographics
{
    [NotMapped]
    public abstract class CensusMetricEntity : BaseDataModel
    {
        /// <summary>
        /// TractID is an alias for GeoID; it is the unique identifier used 
        /// by the US Census as well as NPCOM.dbo.tblShpCensusTract.GeoID.
        /// </summary>
        public string TractID { get; set; }
        /// <summary>
        /// Value is the payload data produced by the Category.MetricType metric.
        /// </summary>
        public decimal? Value { get; set; }
    }
}
