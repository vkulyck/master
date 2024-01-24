// Copyright (c) Mihir Dilip. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace GmWeb.Web.Common.Auth.Services.BypassAuth
{
    /// <summary>
    /// Basic Events.
    /// </summary>
    public class BypassAuthEvents
    {
        public Func<Task> OnSucceeded { get; set; }
        public Func<Task> OnFailed { get; set; }
        public Func<Task> OnChallenge { get; set; }
        public Func<Task> OnForbidden { get; set; }
    }
}
