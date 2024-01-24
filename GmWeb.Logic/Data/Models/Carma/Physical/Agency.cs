using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Carma
{
    [Table("Agencies", Schema = "carma")]
    public class Agency : BaseDataModel
    {
        [Key]
        public int AgencyID { get; set; }
        public string Name { get; set; }
        public byte[] ProfilePicture { get; set; }
        [NotMapped]
        public string ActivityCalendarFilename => $"agencyId={this.AgencyID}.ics";
    }
}