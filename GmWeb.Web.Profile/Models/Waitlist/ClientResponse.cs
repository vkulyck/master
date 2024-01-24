using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Data.Models.Shared;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class ClientResponse : DynamicFieldValue, IViewModel
    {
        public string Guid { get; set; } = ModelExtensions.GenerateGuid();
    }
}