using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace GmWeb.Logic.Data.Context.Identity;

public class GmIdentityContext : IdentityDbContext<GmIdentity, IdentityRole<Guid>, Guid>
{
    protected GmIdentityContext() { }
    public GmIdentityContext(DbContextOptions<GmIdentityContext> options) : base(options) { }
}