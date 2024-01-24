using GmWeb.Logic.Data.Context.Carma;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CarmaUser = GmWeb.Logic.Data.Models.Carma.User;
using CurrentCulture = System.Globalization.CultureInfo;
using GmWeb.Web.Common.Controllers;

namespace GmWeb.Web.RHI.Controllers;
/// <summary>
/// A controller that provides CRUD access and view extensions for a designated CARMA entity.
/// </summary>
public class CarmaController : GmAppController, IDisposable
{
    private readonly CarmaCache _cache;
    protected CarmaCache Cache => _cache;
    protected async Task<int> GetAgencyID()
    {
        var user = await this.GetCarmaUser();
        return user.AgencyID;
    }
    protected async Task<CarmaUser> GetCarmaUser(int? AgencyID = null)
    {
        var identity = await this.GetUserAsync();
        var users = this.Cache.DataContext.Users
            .Include(x => x.ChildConfigs)
        ;
        var carmaUser = await users.SingleOrDefaultAsync(x => x.AccountID == identity.Id);
        // TODO: Users will eventually map to multiple agencies.
        // Once this change has been implemented, the provided AgencyID
        // parameter should be used to specify which of these
        // agencies should be used for the context of the current request.
        // For now, no action is needed.
        if (AgencyID.HasValue && carmaUser.AgencyID != AgencyID)
            throw new ArgumentException($"Invalid AgencyID value of {AgencyID} supplied for UserID {carmaUser.UserID}.");
        return carmaUser;
    }

    protected CarmaController(CarmaContext context, UserManager<GmIdentity> manager)
        : base(manager)
    {
        _cache = new CarmaCache(context);
    }

    new protected virtual void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            this.Cache.Dispose();
        }
    }

    new public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}