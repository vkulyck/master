using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Demographics
{
    [Table("tblBins", Schema = "dmg")]
    public class Bin : BaseDataModel
    {
        /// <summary>
        /// The numeric offset used to partition the BinID.
        /// </summary>
        /// <example>
        /// (int CategoryID, int SequenceNumber) SplitBinID(int BinID)
        ///  {
        ///     return (
        ///         CategoryID: BinID / Offset,
        ///         SequenceNumber: BinID % Offset
        ///     );
        /// }
        /// </example>
        private const int Offset = (int)1E5;

        /// <summary>
        /// The primary key for this entity.
        /// </summary>
        [Key]
        public int BinID { get; set; }
        /// <summary>
        /// A primary key reference to the the demographic category that this bin resides in.
        /// </summary>
        [ForeignKey("Category")]
        public int CategoryID { get; set; } // = BinID / 10^5
        /// <summary>
        /// The demographic category that this bin resides in.
        /// </summary>
        public virtual Category Category { get; set; }
        /// <summary>
        /// An integer identifier from a sequential list that orders this bin and its siblings under a common <see cref="Category"/>.
        /// </summary>
        public int SequenceNumber => this.BinID % Offset;
        /// <summary>
        /// A simple string identifier that is unique among sibling bins under a common <see cref="Category"/>.
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// A human-readable description suitable for display in the UI.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The minimum numeric value for datapoints assigned to this bin.
        /// </summary>
        public decimal? MinValue { get; set; }
        /// <summary>
        /// The maximum numeric value for datapoints assigned to this bin.
        /// </summary>
        public decimal? MaxValue { get; set; }
        /// <summary>
        /// The unique ID for this bin's datafile column as displayed in the first header row.
        /// </summary>
        public string ColumnID { get; set; }
        [InverseProperty("Bin")]
        public virtual ICollection<BinValue> BinValues { get; set; } = new List<BinValue>();

        public Bin() { }
        /// <summary>
        /// The primary constructor used for manual bin instantiation.
        /// </summary>
        /// <param name="category">The parent category for this bin.</param>
        public Bin(Category category)
        {
            int sequence = category.Bins.Count + 1;
            this.Category = category;
            this.CategoryID = category.CategoryID;
            this.BinID = this.CategoryID * Offset + sequence;
            this.Category.Bins.Add(this);
        }

        public override string ToString()
        {
            if (this.MinValue.HasValue && this.MaxValue.HasValue)
                return $"Bin#{this.SequenceNumber:D2}: {this.MinValue:N0} to {this.MaxValue:N0}";
            else if (this.MinValue.HasValue)
                return $"Bin#{this.SequenceNumber:D2}: Less than {this.MinValue:N0}";
            else if (this.MaxValue.HasValue)
                return $"Bin#{this.SequenceNumber:D2}: Greater than {this.MaxValue:N0}";
            else
                return $"Bin#{this.SequenceNumber:D2}: {this.Description}";
        }
    }
}
