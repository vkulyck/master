using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Data.Context.Profile;
using GmWeb.Logic.Data.Models.Waitlists;
using AutoMapper;

namespace GmWeb.Web.Profile.Models.Waitlist
{
    public class DataSource : EditableFlowModelBase
    {
        public int DataSourceID { get; set; }
        public string Name => $"{this.EntityType}: {this.Field}";
        public string Table { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public EntityType EntityType { get; set; }

        public static IEnumerable<DataSource> Options(IMapper mapper)
        {
            using(var cache = new ProfileCache())
            {
                var dbSources = cache.DataSources.ToList();
                foreach (var source in dbSources)
                    yield return mapper.Map<DataSource>(source);
            }
        }
    }
}