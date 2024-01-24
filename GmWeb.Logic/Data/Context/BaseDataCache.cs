using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkConfig = EFCore.BulkExtensions.BulkConfig;

namespace GmWeb.Logic.Data.Context
{
    public abstract class BaseDataCache : IDisposable
    {
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected abstract DbContext _BaseContext { get; }

        public BaseDataCache()
        {
            this.Initialize();
        }
        public virtual void Initialize() => this.InitializeCollectionMap();
        protected virtual void InitializeCollectionMap() { }

        public virtual void Save()
        {
            try
            {
                var changes = this._BaseContext.ChangeTracker.Entries().ToList();
                this._BaseContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Error("Error saving cache.", ex);
                throw;
            }
        }

        public virtual async Task SaveAsync()
        {
            try
            {
                var changes = this._BaseContext.ChangeTracker.Entries().ToList();
                await this._BaseContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("Error saving cache.", ex);
                throw;
            }
        }

        public virtual void Dispose() => this._BaseContext?.Dispose();

        public virtual void BulkInsert<TModel>(IList<TModel> models)
            where TModel : class
        => throw new NotImplementedException();
        public virtual Task BulkInsertAsync<TModel>(IList<TModel> models)
            where TModel : class
        => throw new NotImplementedException();
        public virtual Task BulkInsertAsync<TModel>(IList<TModel> models, BulkConfig bulkConfig)
            where TModel : class
        => throw new NotImplementedException();
        public virtual Task BulkUpdateAsync<TModel>(IList<TModel> models)
            where TModel : class
        => throw new NotImplementedException();
        public virtual Task BulkUpdateAsync<TModel>(IList<TModel> models, BulkConfig bulkConfig)
            where TModel : class
        => throw new NotImplementedException();
    }
    public abstract class BaseDataCache<TContext> : BaseDataCache
        where TContext : BaseDataContext<TContext>, new()
    {
        public TContext DataContext { get; private set; }
        protected override DbContext _BaseContext => this.DataContext;

        public BaseDataCache()
        {
            this.DataContext = new TContext();
        }

        public BaseDataCache(TContext context)
        {
            this.DataContext = context;
        }

        public override void BulkInsert<TModel>(IList<TModel> models)
            where TModel : class
        => this.DataContext.BulkInsert(models);
        public override Task BulkInsertAsync<TModel>(IList<TModel> models)
            where TModel : class
        => this.DataContext.BulkInsertAsync(models);
        public override Task BulkInsertAsync<TModel>(IList<TModel> models, BulkConfig bulkConfig)
            where TModel : class
        => this.DataContext.BulkInsertAsync(models, bulkConfig);
        public override Task BulkUpdateAsync<TModel>(IList<TModel> models)
            where TModel : class
        => this.DataContext.BulkUpdateAsync(models);
        public override Task BulkUpdateAsync<TModel>(IList<TModel> models, BulkConfig bulkConfig)
            where TModel : class
        => this.DataContext.BulkUpdateAsync(models, bulkConfig);
    }
    public abstract class BaseDataCache<TCache, TContext> : BaseDataCache<TContext>
        where TCache : BaseDataCache<TCache, TContext>, new()
        where TContext : BaseDataContext<TContext>, new()
    {
        protected Dictionary<Type, Func<BaseDataCollection>> CollectionMap { get; } = new Dictionary<Type, Func<BaseDataCollection>>();

        public BaseDataCache() { }
        public BaseDataCache(TContext context) : base(context) { }

        public virtual BaseDataCollection GetMatchingCollection(Type tEntity) => this.CollectionMap[tEntity]();
        public virtual BaseDataCollection GetMatchingCollection<TEntity>() => this.GetMatchingCollection(typeof(TEntity));
        public virtual TCollection GetMatchingCollection<TEntity, TCollection>()
            where TCollection : BaseDataCollection
            => this.GetMatchingCollection<TEntity>() as TCollection
        ;
        public virtual TCollection GetMatchingCollection<TCollection>(Type tEntity)
            where TCollection : BaseDataCollection
            => this.GetMatchingCollection(tEntity) as TCollection
        ;

        public virtual void SetModified<TEntity>(TEntity entity)
        {
            this.DataContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
