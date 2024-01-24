using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Config;

public class GmWebOptions : WebOptions
{
    #region Tiers
    public WebAppOptions Identity { get; } = new WebAppOptions();
    public WebAppOptions Api { get; } = new WebAppOptions();
    public WebAppOptions RHI { get; } = new WebAppOptions();
    #endregion

    #region Links
    public WebAction ConfirmEmailChange { get; } = new WebAction();
    public WebAction ConfirmEmail { get; } = new WebAction();
    public WebAction ResetPassword { get; } = new WebAction();
    public WebAction Login { get; } = new WebAction();
    public WebAction Logout { get; } = new WebAction();
    #endregion

    public GmWebOptions()
    {
        this.Initialize(this.Identity);
    }
}
