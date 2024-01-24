using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Profile
{
    [Table("lkpEthnicity")]
    public class Ethnicity : BaseDataModel
    {
        [Key]
        public int Ethcode { get; set; }
        public string EthDescription { get; set; }
        public int EthDisplay { get; set; }
    }
}