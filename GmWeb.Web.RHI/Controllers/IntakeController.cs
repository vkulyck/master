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

namespace GmWeb.Web.RHI.Controllers
{
    public class IntakeController : CarmaController
    {
        private readonly IntakeOptions _options;
        private readonly IWebHostEnvironment _env;
        protected IWebHostEnvironment Env => _env;
        private readonly RedisCache _redis;
        protected RedisCache Redis => _redis;
        public IntakeController(CarmaContext context, GmUserManager manager, IWebHostEnvironment env, RedisCache redis, IOptions<IntakeOptions> options) : base(context, manager) 
        {
            _env = env;
            _redis = redis;
            _options = options.Value;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitData([FromBody] IntakeData requestModel)
        {
            var user = await this.GetCarmaUser();
            var cacheModel = await this.Redis.LoadIntakeDataAsync(user.AccountID.Value);
            this.Mapper.Map(requestModel, cacheModel);
            user.Profile.CurrentIntakeData = null;
            user.Profile.IntakeHistory.Insert(0, cacheModel);
            this.Cache.SetModified(user);
            await this.Redis.ClearIntakeDataAsync(user.AccountID.Value);
            await this.Cache.SaveAsync();
            return this.Success();
        }

        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] IntakeData requestModel)
        {
            var user = await this.GetCarmaUser();
            var cacheModel = await this.Redis.LoadIntakeDataAsync(user.AccountID.Value);
            this.Mapper.Map(requestModel, cacheModel);
            user.Profile.CurrentIntakeData = cacheModel;
            this.Cache.SetModified(user);
            await this.Cache.SaveAsync();
            await this.Redis.StoreIntakeDataAsync(user.AccountID.Value, user.Profile.CurrentIntakeData);
            return this.Success();
        }

        [HttpPost]
        public async Task<IActionResult> CacheData([FromBody] IntakeData requestModel)
        {
            var user = await this.GetCarmaUser();
            var cacheModel = await this.Redis.LoadIntakeDataAsync(user.AccountID.Value);
            this.Mapper.Map(requestModel, cacheModel);
            var success = await _redis.StoreIntakeDataAsync(user.AccountID.Value, cacheModel);
            if(success)
                return this.Success();
            return this.BadRequest("Error caching intake data.");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
            => await this.GetSection(IntakePageType.General);

        [HttpGet]
        [Route("Intake/{pageType}")]
        public async Task<IActionResult> GetSection([FromRoute] IntakePageType pageType)
        {
            this.ViewBag.CurrentPage = IntakePage.GetPage(pageType);

            var user = await this.GetCarmaUser();
            var profileData = await this.Redis.LoadIntakeDataAsync(user.AccountID.Value);
            if (profileData is null)
            {
                profileData = user.Profile.CurrentIntakeData;
                if (profileData is null && this.Env.IsDevelopment())
                    profileData = _options.DefaultIntakeData;
                if (profileData is null)
                    profileData = IntakeData.Empty;
            }
            profileData = Mapper.Map(profileData, IntakeData.Empty);
            await this.Redis.StoreIntakeDataAsync(user.AccountID.Value, profileData);
            return View(viewName: "Index", model: profileData);
        }

        public JsonResult GetPlanTypeList([DataSourceRequest] DataSourceRequest request, string typeID)
        {
            if (typeID == "6")
                return Json(WebControlData.GetMedicalData().ToDataSourceResult(request));
            else if (typeID == "7")
                return Json(WebControlData.GetMedicareData().ToDataSourceResult(request));
            else
                return Json(new Dictionary<int, string>().ToDataSourceResult(request));
        }
        public JsonResult GetAddLangList([DataSourceRequest] DataSourceRequest request, string languageID)
        {
            return Json(WebControlData.GetAddLangData(languageID).ToDataSourceResult(request));
        }
        public JsonResult GetLangList([DataSourceRequest] DataSourceRequest request)
        {
            return Json(WebControlData.GetLangData().ToDataSourceResult(request));
        }

        public JsonResult GetIncomeList([DataSourceRequest] DataSourceRequest request)
        {
            return Json(WebControlData.GetIncomeList().ToDataSourceResult(request));
        }
        public JsonResult GetEducationList([DataSourceRequest] DataSourceRequest request)
        {
            return Json(WebControlData.GetEducationList().ToDataSourceResult(request));
        }
        public JsonResult GetIntList([DataSourceRequest] DataSourceRequest request)
        {
            return Json(WebControlData.GetIntList().ToDataSourceResult(request));
        }

        public JsonResult GetReligionList([DataSourceRequest] DataSourceRequest request)
        {
            return Json(WebControlData.GetReligionList().ToDataSourceResult(request));
        }
        public JsonResult GetEthnicityList([DataSourceRequest] DataSourceRequest request)
        {
            return Json(WebControlData.GetEthnicityList().ToDataSourceResult(request));
        }
        public JsonResult GetRacialList([DataSourceRequest] DataSourceRequest request)
        {
            return Json(WebControlData.GetRacialList().ToDataSourceResult(request));
        }
    }
}
