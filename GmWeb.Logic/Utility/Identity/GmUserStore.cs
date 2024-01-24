using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GmIdentityContext = GmWeb.Logic.Data.Context.Identity.GmIdentityContext;
using GmWeb.Logic.Interfaces;

namespace GmWeb.Logic.Utility.Identity;

public class GmUserStore : UserStore<GmIdentity, IdentityRole<Guid>, GmIdentityContext, Guid>, ICompleteUserStore<GmIdentity>
{
    public GmUserStore(GmIdentityContext context, IdentityErrorDescriber describer = null) 
        : base(context, describer)
    {
    }
}
