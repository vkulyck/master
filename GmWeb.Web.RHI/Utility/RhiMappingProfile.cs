using AutoMapper;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;
using GmWeb.Web.Common.Auth;
using GmWeb.Logic.Utility.Identity.DTO;
using Microsoft.AspNetCore.Identity;
using GmWeb.Web.Common.Utility;

namespace GmWeb.Web.RHI.Utility;
public class RhiMappingProfile : CommonMappingProfile
{
    public RhiMappingProfile()
    {
        // Don't map null members
        this.CreateMap<IntakeData, IntakeData>().ForAllMembers(opts => opts.Condition((src, dest, member) => member is not null));
    }
}