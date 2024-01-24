using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GmWeb.Logic.Utility.Identity;

public class GmRoleStore : RoleStore<GmRole, GmIdentityContext, Guid>
{
    public GmRoleStore(GmIdentityContext context, IdentityErrorDescriber describer = null) : base(context, describer)
    {
    }
}
