using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Utility.Mapping;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Common.Identity;
using AutoMapper;

namespace GmWeb.Web.Profile.Utility
{
    public class ClientServicesMappingFactory : MappingFactory
    {
        public ClientServicesMappingFactory()
        {
            this.AddProfile<CommonMappingProfile>();
            this.AddProfile<IdentityMappingProfile>();
            this.AddProfile<ClientServicesMappingProfile>();
        }
    }
}