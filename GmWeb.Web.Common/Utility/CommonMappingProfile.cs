using AutoMapper;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Data.Models.Identity;
using GmWeb.Logic.Utility.Identity.DTO;
using GmWeb.Web.Common.Models.Carma;
using System.Security.Claims;

namespace GmWeb.Web.Common.Utility
{
    public class CommonMappingProfile : Profile
    {
        public CommonMappingProfile()
        {
            this.CreateMap<User, ApplicationUser>().ReverseMap()
                .ForMember(x => x.AccountID, o => o.MapFrom(x => x.Id))
                .ForMember(x => x.FirstName, o => o.MapFrom(x => x.FirstName))
                .ForMember(x => x.LastName, o => o.MapFrom(x => x.LastName))
                .ForMember(x => x.Phone, o => o.MapFrom(x => x.PhoneNumber))
            ;
            this.CreateMap<GmIdentity, RegisterDTO>().ReverseMap();
            this.CreateMap<Claim, ClaimDTO>().ReverseMap();

            this.CreateMap<User, UserDTO>().ReverseMap();
            this.CreateMap<UserInsertDTO, User>();
            this.CreateMap<UserUpdateDTO, User>()
                .ForMember(dest => dest.FirstName, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.FirstName)))
                .ForMember(dest => dest.LastName, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.LastName)))
                .ForMember(dest => dest.Email, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Email)))
                .ForMember(dest => dest.Gender, opt => opt.Condition(src => src.Gender.HasValue))
                .ForMember(dest => dest.UserRole, opt => opt.Condition(src => src.UserRole.HasValue))
                .ForMember(dest => dest.LanguageCode, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.LanguageCode)))
            ;
        }
    }
}
