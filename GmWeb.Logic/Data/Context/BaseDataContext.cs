using EFCore.BulkExtensions;
using GmWeb.Logic.Utility.Config;
using GmWeb.Logic.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bulk = EFCore.BulkExtensions.DbContextBulkExtensions;
using BulkConfig = EFCore.BulkExtensions.BulkConfig;
using EntityMappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;

namespace GmWeb.Logic.Data.Context
{
    public abstract class BaseDataContext : DbContext
    {
        public static EntityMappingFactory Mapper => EntityMappingFactory.Instance;
        protected IConfiguration _configuration;
        public BaseDataContext()
        {
            this.Database.SetCommandTimeout(3600);
        }
        public BaseDataContext(DbContextOptions options) : base(options) { }
        public int Save() => this.SaveChanges();
        public async Task<int> SaveAsync() => await this.SaveChangesAsync();

        protected override void OnConfiguring(DbContextOptionsBuilder builder) => this.OnConfiguring(builder, default(Action<SqlServerDbContextOptionsBuilder>));
        protected virtual void OnConfiguring(DbContextOptionsBuilder builder, Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction)
        {
#if !NETFRAMEWORK
            if (this._configuration == null)
                this._configuration = new ConfigAccessor();
#endif
            if (!builder.IsConfigured)
            {
                var regex = new Regex($@"^(?:Gm)?(?<name>[A-Z]\w+)Context$", RegexOptions.Compiled);
                string contextName = this.GetType().Name;
                string name = regex.Replace(contextName, "${name}");
#if NETFRAMEWORK
                var connString = System.Configuration.ConfigurationManager.ConnectionStrings["CURRENT_INSTANCE_DB"].ConnectionString;
#else
                string connString = this._configuration.GetConnectionString(name);
#endif
                if (string.IsNullOrWhiteSpace(connString))
                    throw new Exception($"No connection string found with name {name} matching context type {contextName}");
                var defaultConfig = this._configuration.GetSection("DatabaseConnections:Default").Get<DatabaseConnectionOptions>();

                builder.UseLazyLoadingProxies();
                var discriminator = new Regex(@"Data Source=(\w+\.)+test.db");
                bool isSqlLite = discriminator.IsMatch(connString);
                if (isSqlLite)
                    builder.UseSqlite(connString, ctx =>
                    {
                        
                        ctx.CommandTimeout(defaultConfig.CommandTimeout);
                    });
                else
                    builder.UseSqlServer(connString, ctx =>
                    {
                        if(sqlServerOptionsAction != null)
                            sqlServerOptionsAction(ctx);
                        ctx.CommandTimeout(defaultConfig.CommandTimeout);
                    });
            }
        }
    }
    public abstract class BaseDataContext<TContext> : BaseDataContext, IBaseDataContext<TContext>
        where TContext : BaseDataContext<TContext>, new()
    {
        public BaseDataContext() { }
        public BaseDataContext(DbContextOptions<TContext> options) : base(options) { }
        int IBaseDataContext<TContext>.Save() => base.Save();

        private static readonly BulkConfig DefaultBulkConfig = new BulkConfig
        {
            BatchSize = 250,
            PreserveInsertOrder = true
        };

        public void BulkInsert<TModel>(IList<TModel> models)
            where TModel : class
        => this.BulkInsert(models, DefaultBulkConfig);
        public void BulkInsert<TModel>(IList<TModel> models, BulkConfig bulkConfig)
            where TModel : class
        {
            bulkConfig.PropertiesToInclude = typeof(TModel)
                .InferDatabaseProperties()
                .ToList()
            ;
            this.BulkInsert(models, bulkConfig);
        }
        public async Task BulkInsertAsync<TModel>(IList<TModel> models)
            where TModel : class
        => await this.BulkInsertAsync(models, DefaultBulkConfig);
        public async Task BulkInsertAsync<TModel>(IList<TModel> models, BulkConfig bulkConfig)
            where TModel : class
        {
            bulkConfig.PropertiesToInclude = typeof(TModel)
                .InferDatabaseProperties()
                .ToList()
            ;
            await Bulk.BulkInsertAsync(this, models, bulkConfig);
        }
        public async Task BulkUpdateAsync<TModel>(IList<TModel> models)
            where TModel : class
        => await this.BulkUpdateAsync(models, DefaultBulkConfig);
        public async Task BulkUpdateAsync<TModel>(IList<TModel> models, BulkConfig bulkConfig)
            where TModel : class
        {
            bulkConfig.PropertiesToInclude = typeof(TModel)
                .InferDatabaseProperties()
                .ToList()
            ;
            await Bulk.BulkUpdateAsync(this, models, bulkConfig);
        }

        public async Task BulkDeleteAsync<TModel>(IList<TModel> models)
            where TModel : class
        {
            await Bulk.BulkDeleteAsync(this, models);
        }

        public async Task TruncateAsync<TModel>()
            where TModel : class
        {
            await Bulk.TruncateAsync<TModel>(this);
        }
    }
}
