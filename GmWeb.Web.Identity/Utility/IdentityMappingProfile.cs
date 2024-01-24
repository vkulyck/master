using AutoMapper;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Web.Common.Utility;

namespace GmWeb.Web.Identity.Data
{
    public class IdentityMappingProfile : CommonMappingProfile
    {
        public IdentityMappingProfile()
        {
            this.CreateMap<User, GmIdentity>().ReverseMap();
        }
    }
}
