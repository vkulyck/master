using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using GmWeb.Logic.Interfaces;
using GmWeb.Logic.Enums;
using ILType = GmWeb.Logic.Enums.IncomeLevel;
using GmWeb.Logic.Utility.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Web.Demographics.Logic.DataModels
{
    public class ClientDetail
    {
        public static readonly Func<string, ILType?> ParseIncomeLevel = (string RawIncomeLevel) =>
            RawIncomeLevel == "Extremely Low Income" ? ILType.ExtremeLow :
            RawIncomeLevel == "Low Income" ? ILType.Low :
            RawIncomeLevel == "Moderate Income" ? ILType.Moderate :
            RawIncomeLevel == "Above Moderate Income" ? ILType.AboveModerate :
            default(ILType?)
        ;

        public int ClientID { get; set; }
        public int AgencyID { get; set; }
        public string ActivityID { get; set; }
        public int CategoryID { get; set; }
        public int ProjectID { get; set; }
        public int WorkPlanID { get; set; }
        public int ClientCategoryID { get; set; }
        public int ClientCategoryDateID { get; set; }

        public int? ProjectYear { get; set; }
        public string Program { get; set; }
        public string AgencyName { get; set; }
        public int? WorkActivityType { get; set; }
        public string ActivityDescription { get; set; }
        public bool Assigned { get; set; }
        public string HUDCode { get; set; }
        public DateTime ScheduledDate { get; set; }
        
        public IncomeLevel? IncomeLevel { get; set; }
        public int? NumberInFamily { get; set; }
        public int? NumberInHousehold { get; set; }
        public string Ethnicity { get; set; }
        public int? Age { get; set; }

        public int CategoryMonth { get; set; }
        public int CategoryQuarter { get; set; }
        public int CategoryYear { get; set; }

        public Gender? Gender { get; set; }
        public HeadOfHousehold? HeadOfHouseholdType { get; set; }

        public virtual string GetChartCategory() => throw new NotImplementedException();
    }
}