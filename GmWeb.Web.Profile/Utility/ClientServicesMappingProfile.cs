using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MappingProfile = AutoMapper.Profile;
using GmWeb.Web.Common.Utility;
using DataModels = GmWeb.Logic.Data.Models;
using ViewModels = GmWeb.Web.Profile.Models;

namespace GmWeb.Web.Profile.Utility
{
    public class ClientServicesMappingProfile : MappingProfile
    {
        public ClientServicesMappingProfile()
        {
            this.CreateMap<DataModels.Waitlists.FlowLink, ViewModels.Waitlist.FlowLink>().ReverseMap();
            this.CreateMap<DataModels.Waitlists.CategoryData, ViewModels.Waitlist.CategoryData>().ReverseMap();
            this.CreateMap<DataModels.Waitlists.ClientQuery, ViewModels.Waitlist.ClientQuery>().ReverseMap();
            this.CreateMap<DataModels.Waitlists.DataSource, ViewModels.Waitlist.DataSource>().ReverseMap();
            this.CreateMap<DataModels.Waitlists.ResultSet, ViewModels.Waitlist.ResultSet>().ReverseMap();
            this.CreateMap<DataModels.Waitlists.Flow, ViewModels.Waitlist.Flow>().ReverseMap();
            this.CreateMap<DataModels.Waitlists.FlowStep, ViewModels.Waitlist.FlowStep>().ReverseMap();
            this.CreateMap<DataModels.Waitlists.FilterResultData, ViewModels.Waitlist.FilterResultData>().ReverseMap();
            this.CreateMap<DataModels.Shared.DynamicFieldValue, ViewModels.Waitlist.ClientResponse>().ReverseMap();
            this.CreateMap<DataModels.Profile.Client, ViewModels.Waitlist.Client>().ReverseMap();
        }
    }
}