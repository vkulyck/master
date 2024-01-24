using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Web.Demographics.Logic.DataModels;
using GmWeb.Web.Common.Models;

namespace GmWeb.Web.Demographics.ViewModels
{
    public class ProgramViewModel : BasePageViewModel
    {
        public List<string> ActivityIDs { get; set; }
    }
}