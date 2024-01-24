using GmWeb.Logic.Data.Models;
using GmWeb.Logic.Utility.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Context
{
    public abstract class BaseDataCollection { }
    public abstract partial class BaseDataCollection<TEntity> : BaseDataCollection, IQueryable<TEntity>
        where TEntity : class, new()
    {
        public static EntityMappingFactory Mapper => EntityMappingFactory.Instance;

        #region IEnumerable and IQueryable Members
        public IEnumerator<TEntity> GetEnumerator() => this.EntitySet.AsQueryable().GetEnumerator();

        public Expression Expression => this.EntitySet.AsQueryable().Expression;

        public Type ElementType => this.EntitySet.AsQueryable().ElementType;

        public IQueryProvider Provider => this.EntitySet.AsQueryable().Provider;

        #endregion

        public virtual TEntity Insert(TEntity entity)
        {
            this.EntitySet.Add(entity);
            return entity;
        }

        public virtual TEntity Create() => this.Insert(new());
        public virtual TEntity Create(Action<TEntity> initializer)
        {
            var entity = new TEntity();
            if (initializer != null)
                initializer(entity);
            return this.Insert(entity);
        }
        public virtual TEntity Create(Func<TEntity> generator) => this.Insert(generator());

        public void AddRange(IEnumerable<TEntity> entities)
        {
            // Iterate and add one-by-one because DbSet.AddRange doesn't respect enumeration order.
            foreach (var e in entities)
                this.Add(e);
        }

        public virtual void Add(TEntity entity) => this.EntitySet.Add(entity);
        public virtual async Task AddAsync(TEntity entity) => await this.EntitySet.AddAsync(entity);
        public void Attach(TEntity entity) => this.EntitySet.Attach(entity);
        public virtual void Delete(TEntity entity) => this.EntitySet.Remove(entity);
        public abstract EntityEntry<TEntity> Entry(TEntity entity);
        public void RemoveRange(IEnumerable<TEntity> entities) => this.EntitySet.RemoveRange(entities);
        public void RemoveAll() => this.EntitySet.RemoveRange(this.EntitySet);
        public void RemoveWhere(Expression<Func<TEntity, bool>> predicate) => this.EntitySet.RemoveRange(this.EntitySet.Where(predicate));
        public void Save() => this.SaveAsync().Wait();
        public abstract Task SaveAsync();
        public virtual EntityEntry<TEntity> Update(TEntity entity) => this.EntitySet.Update(entity);

        protected abstract Task UpdateSingleAsync(TEntity source);

        public async Task<EntityEntry<TEntity>> UpdateSingleAsync<TSource,TKey>(TSource source)
            where TSource : IPrimaryKeyModel<TKey>
            where TKey : struct
        {
            var target = this.EntitySet.Find(source.PrimaryKey);
            EntityMappingFactory.Instance.Map(source, target);
            await this.UpdateSingleAsync(target);
            var entry = this.Entry(target);
            entry.Reload();
            return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var enumerable = (this.EntitySet as IEnumerable);
            return enumerable.GetEnumerator();
        }

        public virtual DbSet<TEntity> EntitySet => throw new NotImplementedException();
    }
    public abstract class BaseDataCollection<TEntity, TCollection, TCache, TContext> : BaseDataCollection<TEntity>
        where TEntity : BaseDataModel, new()
        where TCollection : BaseDataCollection<TEntity, TCollection, TCache, TContext>
        where TCache : BaseDataCache<TCache, TContext>, new()
        where TContext : BaseDataContext<TContext>, new()
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TCache Cache { get; set; }

        public TContext DataContext => this.Cache.DataContext;

        public BaseDataCollection(TCache cache)
        {
            this.Cache = cache;
        }

        public virtual TCollection Initialize() => (TCollection)this;
        public override EntityEntry<TEntity> Entry(TEntity entity) => this.DataContext.Entry(entity);

        public override TEntity Create(Action<TEntity> initializer)
        {
            var entity = base.Create(initializer);
            return this.Contextualize(entity);
        }

        public override TEntity Create(Func<TEntity> generator)
        {
            var entity = base.Create(generator);
            return this.Contextualize(entity);
        }

        protected TEntity Contextualize(TEntity entity)
        {
            var contextEntity = entity as ContextualDataModel<TContext>;
            if (contextEntity != null)
                contextEntity.PopulateReferences(this.DataContext);
            return entity;
        }

        public override Task SaveAsync() => this.Cache.SaveAsync();
        protected override async Task UpdateSingleAsync(TEntity source)
        {
            using (var cache = new TCache())
            {
                var collection = cache.GetMatchingCollection<TEntity, TCollection>();
                var targetEntry = collection.Update(source);
                if (targetEntry.State == EntityState.Detached)
                    collection.Attach(source);
                else if (targetEntry.State == EntityState.Deleted)
                    throw new Exception($"Cannot update a deleted entity.");
                else if (targetEntry.State == EntityState.Unchanged)
                    targetEntry.State = EntityState.Modified;
                await collection.SaveAsync();
            }
        }
    }
}
