using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Web.Common;
using GmWeb.Web.Common.Utility;
using GmWeb.Web.Common.RazorControls; 

namespace GmWeb.Web.Common.RazorControls.ModelEditors
{
    public class MeteredPasswordOptions<TModel> : ModelFormItemOptions<TModel, string, MeteredPasswordOptions<TModel>>
        where TModel : class, new()
    {
        public string ErrorTooltipClass { get; set; }
        public string StrengthProgressDisplayId { get; set; }
    }
}
