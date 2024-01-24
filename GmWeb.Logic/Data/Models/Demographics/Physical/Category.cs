using GmWeb.Logic.Data.Annotations;
using GmWeb.Logic.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Demographics
{
    [Table("tblCategories", Schema = "dmg")]
    public class Category : BaseDataModel
    {
        /// <summary>
        /// Foreign key reference for <see cref="Category">.</see>
        /// </summary>
        [Key]
        public int CategoryID { get; set; }
        /// <summary>
        /// MetricType defines the metric used to produce the MetricValue 
        /// property of BinValue / CategoryValue.
        /// </summary>
        [ForeignKey("Dataset")]
        public int DatasetID { get; set; }
        /// <summary>
        /// The dataset from which this category is sourced.
        /// </summary>
        public virtual Dataset Dataset { get; set; }

        [SqlDataType(System.Data.SqlDbType.Int)]
        public MetricType MetricType { get; set; }
        public string Name { get; set; }
        [InverseProperty("Category")]
        public virtual ICollection<Bin> Bins { get; set; } = new List<Bin>();
        public string Identifier => this.Name.Replace(" ", "");

        public override string ToString() => $"{this.CategoryID}: {this.Name} <- {this.Dataset}";
    }
}
