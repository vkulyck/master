using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using GmWeb.Web.Demographics.Logic.Data.Context;
using GmWeb.Web.Demographics.Logic;
using GmWeb.Web.Demographics.Logic.DataModels;
using GmWeb.Logic.Data.Models.Profile;
using GmWeb.Web.Demographics.ViewModels;
using GmWeb.Web.Demographics.ViewModels.Geo;
using Microsoft.EntityFrameworkCore;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Enums;

namespace GmWeb.Web.Demographics.Controllers
{
    [GmWeb.Web.Common.Utility.QueryStringRequestFilter("SelectedTab", "int", "ActivityFilter", "string")]
    public class ProgramController : BaseClientServicesController<ProgramViewModel>
    {
        public const string DefaultActivityFilter = "5.1";
        public string ActivityFilter
        {
            get => this.Session["ActivityFilter"]?.ToString();
            set => this.Session["ActivityFilter"] = value;
        }

        public ActionResult Index()
        {
            var model = this.CreateViewModel();
            using (var context = new DemographicsContext())
            {
                model.ActivityIDs = context.Activities.Select(x => x.ActivityID).Distinct().ToList();
            }
            ViewBag.ActivityFilter = this.ActivityFilter = ViewBag.ActivityFilter ?? DefaultActivityFilter;
            return View(model);
        }

        protected ActionResult _ClientDataChart<T>(
            string title,
            Func<DemographicsContext, IQueryable<T>> query, 
            Expression<Func<ClientDetail,string>> categorySelector
        ) where T : ClientDetail
        {
            PieChartModel model;
            using (var context = new DemographicsContext())
            {
                var data = query(context);
                model = PieChartModel.FromClientData(data, categorySelector);
                model.Title = title;
            }
            return this.PartialView("_ParticipantPieChart", model);
        }
        public ActionResult _ClientAgeChart()
        {
            Func<DemographicsContext, IQueryable<ClientAge>> query = (DemographicsContext context) => context.ClientAges
                .Where(x => x.Age.HasValue)
                .Where(x => x.ActivityID.Contains(this.ActivityFilter))
            ;
            return _ClientDataChart("Client Age", query, ClientAge.CategorySelectorExpression);
        }
        public ActionResult _ClientIncomeChart()
        {
            Func<DemographicsContext, IQueryable<ClientIncomeLevel>> query = (DemographicsContext context) => context.ClientIncomeLevels
                .Where(x => x.IncomeLevel.HasValue)
                .Where(x => x.ActivityID.Contains(this.ActivityFilter))
            ;
            return _ClientDataChart("Client Income", query, ClientIncomeLevel.CategorySelectorExpression);
        }
        public ActionResult _ClientEthnicityChart()
        {
            Func<DemographicsContext, IQueryable<ClientEthnicity>> query = (DemographicsContext context) => context.ClientEthnicities
                .Where(x => !string.IsNullOrWhiteSpace(x.Ethnicity))
                .Where(x => x.ActivityID.Contains(this.ActivityFilter))
            ;
            return _ClientDataChart("Client Ethnicity", query, ClientEthnicity.CategorySelectorExpression);
        }

        public ActionResult _PCMChart()
        {
            using (var context = new DemographicsContext())
            {
                var details = context.ParticipantCategoryMonths
                    .Where(x => x.ActivityID.Contains(this.ActivityFilter))
                ;
                Expression<Func<ClientDetail, MultiSeriesViewModel.DataIndex>> selector =
                    (ClientDetail cd) =>
                    new MultiSeriesViewModel.DataIndex(cd.CategoryMonth.ToString(), cd.ActivityDescription, cd.CategoryMonth)
                ;
                var model = MultiSeriesViewModel.FromClientData(details, selector);
                model.CategoryAxisTitle = "Month";
                model.ValueAxisTitle = "Participants";
                model.Title = "Participants by Category-Month";
                return this.PartialView("_ParticipantLineChart", model);
            }
        }

        public ActionResult _ParticipantHoHGenderChart()
        {
            using (var context = new DemographicsContext())
            {
                var details = context.ParticipantCategoryMonths
                    .Where(x => x.HeadOfHouseholdType.HasValue)
                    .Where(x => x.Gender.HasValue)
                    .Where(x => x.ActivityID.Contains(this.ActivityFilter))
                ;
                Expression<Func<ClientDetail, MultiSeriesViewModel.DataIndex>> selector =
                    (ClientDetail cd) =>
                    new MultiSeriesViewModel.DataIndex(cd.HeadOfHouseholdType.ToString(), cd.Gender.ToString(), (int)cd.HeadOfHouseholdType)
                ;
                var model = MultiSeriesViewModel.FromClientData(details, selector);
                model.CategoryAxisTitle = "Head of Household Status";
                model.ValueAxisTitle = "Participants";
                model.Title = "Participants by Head of Household and Gender";
                return this.PartialView("_ParticipantStackedBarChart", model);
            }
        }

        public ActionResult _ParticipantEthnicityBreakdownChart()
        {
            using (var context = new DemographicsContext())
            {
                var details = context.ParticipantCategoryMonths
                    .Where(x => !string.IsNullOrWhiteSpace(x.Ethnicity))
                    .Where(x => x.ActivityID.Contains(this.ActivityFilter))
                ;
                Expression<Func<ClientDetail, MultiSeriesViewModel.DataIndex>> selector =
                    (ClientDetail cd) =>
                    new MultiSeriesViewModel.DataIndex(cd.Ethnicity.ToString(), "Participants", 0)
                ;
                var model = MultiSeriesViewModel.FromClientData(details, selector);
                model.CategoryAxisTitle = "Income Level";
                model.ValueAxisTitle = "Participants";
                model.Title = "Participant Ethnicity Breakdown";

                // TODO: Replace sample data
                model.Series.Add(new MultiSeriesViewModel.DataSeries
                {
                    Categories = model.Series.Last().Categories.ToList(),
                    Data = model.Series.Last().Data.Select(x => x / 2).ToList(),
                    SeriesName = "City"
                });
                return this.PartialView("_ParticipantGroupedBarChart", model);
            }
        }
        public static readonly Func<IncomeLevel, string> ICLDisplay = (IncomeLevel x) =>
             x == IncomeLevel.ExtremeLow ? "Extremely Low"
             : x == IncomeLevel.Low ? "Low"
             : x == IncomeLevel.Moderate ? "Moderate"
             : x == IncomeLevel.AboveModerate ? "Above Moderate"
             : ""
        ;

        public ActionResult _ParticipantIncomeBreakdownChart()
        {
            using (var context = new DemographicsContext())
            {
                var details = context.ParticipantCategoryMonths
                    .Where(x => x.IncomeLevel.HasValue)
                    .Where(x => x.ActivityID.Contains(this.ActivityFilter))
                ;
                Expression<Func<ClientDetail, MultiSeriesViewModel.DataIndex>> selector =
                    (ClientDetail cd) =>
                    new MultiSeriesViewModel.DataIndex(ICLDisplay(cd.IncomeLevel.Value), "Participants", (int)cd.IncomeLevel)
                ;
                var model = MultiSeriesViewModel.FromClientData(details, selector);
                model.CategoryAxisTitle = "Income Level";
                model.ValueAxisTitle = "Participants";
                model.Title = "Participant Income Breakdown";

                var city = context.CityIncomeLevels
                    .Where(x => x.StateCode == "06")
                    .Where(x => x.CountyCode == "075")
                    .OrderBy(x => ICLDisplay(x.IncomeLevel))
                    .ToList()
                ;

                model.Series.Add(new MultiSeriesViewModel.DataSeries
                {
                    Categories = model.Series.Last().Categories.ToList(),
                    Data = city.Select(x => (double)x.HouseholdCount).ToList(),
                    SeriesName = "City"
                });
                return this.PartialView("_ParticipantGroupedBarChart", model);
            }
        }

        public ActionResult _LivingWageIncomeGapChart()
        {
            // TODO: Fill in and pick a better chart type
            /*
             * category: income level
             * s1: income gap = (living wage value - hud_income_amount)
             * s2: household count 
            */
            using (var context = new DemographicsContext())
            {
                var details = context.ClientWageGaps
                    .Where(x => x.ActivityID.Contains(this.ActivityFilter))
                    .GroupBy(x => new { x.IncomeLevel, x.WageGap })
                    .OrderBy(x => (int)x.Key.IncomeLevel)
                    .Select(x => new
                    {
                        IncomeLevel = ICLDisplay(x.Key.IncomeLevel),
                        x.Key.WageGap,
                        HouseholdCount = x.Select(y => y.HouseholdCount).Sum()
                    })
                    .ToList()
                ;
                var categories = details.Select(x => x.IncomeLevel).ToList();
                var model = new MultiSeriesViewModel();
                model.Series.Add(new MultiSeriesViewModel.DataSeries
                {
                    Categories = categories,
                    Data = details.Select(x => (double)x.HouseholdCount).ToList(),
                    SeriesName = "Households"
                });
                model.Series.Add(new MultiSeriesViewModel.DataSeries
                {
                    Categories = categories,
                    Data = details.Select(x => (double)x.WageGap).ToList(),
                    SeriesName = "Gap"
                });
                model.CategoryAxisTitle = "Income Level";
                model.Title = "Participant Income Gaps";
                model.Categories = categories;
                model.SeriesNames = model.Series.Select(x => x.SeriesName);
                return this.PartialView("_DualValueGroupedBarChart", model);
            }
        }
    }
}