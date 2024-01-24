namespace GmWeb.Web.Api.Models.Common
{
    public class AgencyDTO
    {
        public int AgencyID { get; set; }
        public string Name { get; set; }

    }

    public class AgencyDetailsDTO : AgencyDTO
    {
        public byte[] ProfilePicture { get; set; }
    }
}
