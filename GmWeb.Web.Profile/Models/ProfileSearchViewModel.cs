using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SearchType = GmWeb.Web.Profile.Models.ProfileSearch.SearchType;

namespace GmWeb.Web.Profile.Models
{
    public class ProfileSearchViewModel : GmWeb.Web.Common.Models.BasePageViewModel
    {
        public string Keyword { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool EnablePrevButton { get; set; }
        public bool EnableNextButton { get; set; }
        public int ClientDataTypeID { get; set; }
        public SearchType SearchType { get; set; }
        public bool ProfileSearchChecked => this.SearchType == SearchType.Profiles;
        public bool KeywordSearchChecked => this.SearchType == SearchType.Keywords;
    }
}