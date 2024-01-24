using GmWeb.Logic.Data.Models.Shared;
using GmWeb.Logic.Utility.Extensions.Collections;
using GmWeb.Logic.Utility.Extensions.Resources;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GmWeb.Logic.Data.Context.Lookup
{
    public partial class LookupCache : BaseDataCache<LookupCache, LookupContext>
    {
        private static LookupCache _instance;
        public static LookupCache Instance => _instance ?? (_instance = new LookupCache());
        protected Dictionary<string, List<LookupOption>> LookupOptionMap { get; } = new Dictionary<string, List<LookupOption>>();
        protected Dictionary<string, LookupTableConfig> LookupTableConfigs { get; private set; } = new Dictionary<string, LookupTableConfig>();

        public override void Initialize()
        {
            string json = ResourceExtensions.GetEmbeddedResource("Data/Configuration/LookupOptionTables.json");
            var configs = JsonConvert.DeserializeObject<List<LookupTableConfig>>(json);
            this.LookupTableConfigs = this.LookupTableConfigs.MergeLeft(configs.ToDictionary(x => x.TableName));
            base.Initialize();
        }

        public IEnumerable<LookupOption> GetLookupOptions(string table)
        {
            if (this.LookupOptionMap.TryGetValue(table, out var options))
            {
                return options;
            }
            else if (this.LookupTableConfigs.TryGetValue(table, out var config))
            {
                string sql = $"SELECT {config.PrimaryKeyColumn} AS ID, {config.DescriptionColumn} AS Description FROM {table}";
                // options = this.DataContext.LookupOptions.FromSql(sql).ToList(); // .NET Framework
                options = this.DataContext.LookupOptions.FromSqlRaw(sql).ToList(); // .NET Standard
                this.LookupOptionMap[table] = options;
                return options;
            }
            throw new Exception($"No lookup options found for table '{table}'");
        }
    }
}
