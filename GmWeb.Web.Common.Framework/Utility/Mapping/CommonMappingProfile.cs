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
    public class CommonMappingProfile : Profile
    {
        public CommonMappingProfile()
        {
            this.AddConditionalObjectMapper().Conventions.Add(typePair => typePair.SourceType.Namespace != "System.Data.Entity.DynamicProxies");
            this.ValidateInlineMaps = false;
        }
    }
}