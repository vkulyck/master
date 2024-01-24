using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using GmWeb.Web.Common.Identity;
using DataModels = GmWeb.Logic.Data.Models;
using ViewModels = GmWeb.Web.Common.Models;

namespace GmWeb.Web.Common.Utility
{
    public class IdentityMappingProfile : IdentityMappingProfile<GmIdentity> { }
    public class IdentityMappingProfile<TUser> : Profile where TUser : GmIdentity
    {
        public IdentityMappingProfile()
        {
            this.CreateMap<TUser, DataModels.Profile.User>().ReverseMap();
            this.CreateMap<TUser, DataModels.Profile.Client>().ReverseMap();
            this.CreateMap<TUser, DataModels.Profile.UserAccount>().ReverseMap();
            this.CreateMap<TUser, DataModels.Profile.ClientAccount>().ReverseMap();
            this.CreateMap<TUser, DataModels.Profile.BaseAccount>().ReverseMap();
        }
    }
}