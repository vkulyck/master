using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using EFCore.BulkExtensions;

namespace GmWeb.Logic.Data.Context
{
    public interface IBaseDataContext : IDisposable
    {
    }
    public interface IBaseDataContext<TContext> : IBaseDataContext where TContext : IBaseDataContext<TContext>
    {
        int Save();
        void BulkInsert<TModel>(IList<TModel> models) where TModel : class;
        void BulkInsert<TModel>(IList<TModel> models, BulkConfig bulkConfig) where TModel : class;
        Task BulkInsertAsync<TModel>(IList<TModel> models) where TModel : class;
        Task BulkInsertAsync<TModel>(IList<TModel> models, BulkConfig bulkConfig) where TModel : class;
        Task BulkUpdateAsync<TModel>(IList<TModel> models) where TModel : class;
        Task BulkUpdateAsync<TModel>(IList<TModel> models, BulkConfig bulkConfig) where TModel : class;
        Task BulkDeleteAsync<TModel>(IList<TModel> models) where TModel : class;
        Task TruncateAsync<TModel>() where TModel : class;
    }

    public static class IBaseDataContextExtensions
    {
        public static void Initialize<TContext>(this TContext context)
            where TContext : DbContext, IBaseDataContext => context.Database.SetCommandTimeout(3600);
        public static DbConnection GetConnection<TContext>(this TContext context)
            where TContext : DbContext, IBaseDataContext
        => context.Database.GetDbConnection();

        public static int Save<TContext>(this TContext context)
            where TContext : DbContext, IBaseDataContext
        => context.SaveChanges();
        public static async Task<int> SaveAsync<TContext>(this TContext context)
            where TContext : DbContext, IBaseDataContext
        => await context.SaveChangesAsync();

        public static TContext CreateNew<TContext>(this TContext context)
            where TContext : IBaseDataContext<TContext>
        => (TContext)Activator.CreateInstance(typeof(TContext));

        public static TContext CreateNew<TContext>(this TContext context, DbContextOptions options)
            where TContext : IBaseDataContext<TContext>
        => (TContext)Activator.CreateInstance(typeof(TContext), new object[] { options });
    }
}
