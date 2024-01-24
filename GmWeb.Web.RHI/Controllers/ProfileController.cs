using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Algenta.Globalization.LanguageTags;

using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Utility.Redis;
using GmWeb.Logic.Utility.Extensions.Enums;
using GmWeb.Logic.Utility.Extensions.Reflection;
using GmWeb.Web.Common.Controllers;
using GmWeb.Logic.Data.Models.Carma.ExtendedData.Intake;
using GmWeb.Web.RHI.Utility;

namespace GmWeb.Web.RHI.Controllers;

public class ProfileController : CarmaController
{
    private readonly IWebHostEnvironment _env;
    protected IWebHostEnvironment Env => _env;
    private readonly RedisCache _redis;
    protected RedisCache Redis => _redis;
    public ProfileController(CarmaContext context, GmUserManager manager, IWebHostEnvironment env, RedisCache redis) : base(context, manager) 
    {
        _env = env;
        _redis = redis;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return this.View();
    }
}
