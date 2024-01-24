// Copyright (c) Mihir Dilip. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Authentication;

namespace GmWeb.Web.Common.Auth.Services.BypassAuth
{
    /// <summary>
    /// Options used to configure basic authentication.
    /// </summary>
    public class BypassAuthOptions : AuthenticationSchemeOptions
    {
        public BypassAuthOptions()
        {
            base.Events = new BypassAuthEvents();
        }

        public new BypassAuthEvents Events
        {
            get => (BypassAuthEvents)base.Events;
            set => base.Events = value;
        }

        public bool Enabled { get; set; }
        public string AuthenticationEmail { get; set; }
        public bool IgnoreAuthenticationIfAllowAnonymous { get; set; }
        public bool IgnoreAuthenticationIfAlreadyAuthenticated { get; set; }
    }
}
