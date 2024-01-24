using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Web.Common.Identity;
using GmWeb.Logic.Utility.Mapping;

namespace GmWeb.Web.Common.Utility
{
    public class IdentityMappingFactory<TUser> : MappingFactory
        where TUser : GmIdentity
    {
        public IdentityMappingFactory()
        {
            this.AddProfile<EntityMappingProfile>();
            this.AddProfile<CommonMappingProfile>();
            this.AddProfile<IdentityMappingProfile>();
            if(typeof(TUser) != typeof(GmIdentity))
                this.AddProfile<IdentityMappingProfile<TUser>>();
        }
    }
}