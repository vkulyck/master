using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using DataModels = GmWeb.Logic.Data.Models;
using MappingProfile = AutoMapper.Profile;

namespace GmWeb.Web.Demographics.Helpers
{
    public static class ClientServicesMapping
    {
        public class ClientServicesMappingProfile : MappingProfile
        {
            public ClientServicesMappingProfile()
            {
                this.AddConditionalObjectMapper().Conventions.Add(typePair => typePair.SourceType.Namespace != "System.Data.Entity.DynamicProxies");
            }
        }

        public static MapperConfiguration Config { get; private set; }
        public static IMapper Mapper { get; private set; }

        public static T Map<T>(object model)
        {
            return Mapper.Map<T>(model);
        }

        public static List<T> Map<T>(IEnumerable collection)
        {
            var items = new List<T>();
            foreach(var model in collection)
            {
                var mapped = Mapper.Map<T>(model);
                items.Add(mapped);
            }
            return items;
        }

        static ClientServicesMapping()
        {
            Config = new MapperConfiguration(cfg => {
                cfg.AddProfile<ClientServicesMappingProfile>();
                cfg.ValidateInlineMaps = false;
            });
            Mapper = Config.CreateMapper();
        }
    }
}