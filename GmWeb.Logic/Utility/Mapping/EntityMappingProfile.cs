using AutoMapper;
using GmWeb.Logic.Utility.Extensions.Reflection;
using DataModels = GmWeb.Logic.Data.Models;
using IdentityModels = GmWeb.Logic.Utility.Identity.DTO;

namespace GmWeb.Logic.Utility.Mapping
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            var modelTypes = typeof(DataModels.BaseDataModel).GetDescendants();
            foreach (var mt in modelTypes)
            {
                this.CreateMap(sourceType: mt, destinationType: mt).ReverseMap();
            }
            this.CreateMap<System.Security.Claims.Claim, IdentityModels.ClaimDTO>();
        }
    }
}