using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using GmWeb.Logic.Utility.Config;
using GmWeb.Web.Common.Controllers;
using Microsoft.AspNetCore.WebUtilities;
using GmWeb.Logic.Utility.Extensions.Http;

namespace GmWeb.Web.RHI.Controllers;
public class AccountController : GmAppController
{
    private readonly GmWebOptions _options;
    public AccountController(IOptions<GmWebOptions> options, GmUserManager manager) : base(manager)
    {
        _options = options.Value;
    }
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        if (returnUrl == null)
            returnUrl = this._options.RHI;
        else
            returnUrl = new Uri(this._options.RHI.BaseUri, returnUrl).AbsoluteUri;
        var callback = _options.Login.WithQuery("ReturnUrl", returnUrl);
        return Redirect(callback.Url);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Logout(string returnUrl = null)
    {
        if (returnUrl == null)
            returnUrl = this._options.RHI;
        else
            returnUrl = new Uri(this._options.RHI.BaseUri, returnUrl).AbsoluteUri;
        var callback = _options.Logout.WithQuery("ReturnUrl", returnUrl);
        return Redirect(callback.Url);
    }
}
