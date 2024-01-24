


namespace GmWeb.Logic.Data.Context;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public partial class BaseDataCollection<TEntity>
{
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AnyAsync(cancellationToken);
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AnyAsync(predicate, cancellationToken);
    public Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AllAsync(predicate, cancellationToken);
    public Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().CountAsync(cancellationToken);
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().CountAsync(predicate, cancellationToken);
    public Task<long> LongCountAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().LongCountAsync(cancellationToken);
    public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().LongCountAsync(predicate, cancellationToken);
    public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().FirstAsync(cancellationToken);
    public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().FirstAsync(predicate, cancellationToken);
    public Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().FirstOrDefaultAsync(cancellationToken);
    public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().FirstOrDefaultAsync(predicate, cancellationToken);
    public Task<TEntity> LastAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().LastAsync(cancellationToken);
    public Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().LastAsync(predicate, cancellationToken);
    public Task<TEntity> LastOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().LastOrDefaultAsync(cancellationToken);
    public Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().LastOrDefaultAsync(predicate, cancellationToken);
    public Task<TEntity> SingleAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SingleAsync(cancellationToken);
    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SingleAsync(predicate, cancellationToken);
    public Task<TEntity> SingleOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SingleOrDefaultAsync(cancellationToken);
    public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SingleOrDefaultAsync(predicate, cancellationToken);
    public Task<TEntity> MinAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().MinAsync(cancellationToken);
    public Task<TResult> MinAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().MinAsync(selector, cancellationToken);
    public Task<TEntity> MaxAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().MaxAsync(cancellationToken);
    public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().MaxAsync(selector, cancellationToken);
    public Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SumAsync(selector, cancellationToken);
    public Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SumAsync(selector, cancellationToken);
    public Task<int> SumAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SumAsync(selector, cancellationToken);
    public Task<int?> SumAsync(Expression<Func<TEntity, int?>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SumAsync(selector, cancellationToken);
    public Task<long> SumAsync(Expression<Func<TEntity, long>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SumAsync(selector, cancellationToken);
    public Task<long?> SumAsync(Expression<Func<TEntity, long?>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SumAsync(selector, cancellationToken);
    public Task<double> SumAsync(Expression<Func<TEntity, double>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SumAsync(selector, cancellationToken);
    public Task<double?> SumAsync(Expression<Func<TEntity, double?>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SumAsync(selector, cancellationToken);
    public Task<float> SumAsync(Expression<Func<TEntity, float>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SumAsync(selector, cancellationToken);
    public Task<float?> SumAsync(Expression<Func<TEntity, float?>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().SumAsync(selector, cancellationToken);
    public Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AverageAsync(selector, cancellationToken);
    public Task<decimal?> AverageAsync(Expression<Func<TEntity, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AverageAsync(selector, cancellationToken);
    public Task<double> AverageAsync(Expression<Func<TEntity, int>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AverageAsync(selector, cancellationToken);
    public Task<double?> AverageAsync(Expression<Func<TEntity, int?>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AverageAsync(selector, cancellationToken);
    public Task<double> AverageAsync(Expression<Func<TEntity, long>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AverageAsync(selector, cancellationToken);
    public Task<double?> AverageAsync(Expression<Func<TEntity, long?>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AverageAsync(selector, cancellationToken);
    public Task<double> AverageAsync(Expression<Func<TEntity, double>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AverageAsync(selector, cancellationToken);
    public Task<double?> AverageAsync(Expression<Func<TEntity, double?>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AverageAsync(selector, cancellationToken);
    public Task<float> AverageAsync(Expression<Func<TEntity, float>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AverageAsync(selector, cancellationToken);
    public Task<float?> AverageAsync(Expression<Func<TEntity, float?>> selector, CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().AverageAsync(selector, cancellationToken);
    public Task<bool> ContainsAsync(TEntity item, CancellationToken cancellationToken = default(CancellationToken)) 
        => this.EntitySet.AsQueryable().ContainsAsync(item, cancellationToken);
    public Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().ToListAsync(cancellationToken);
    public Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default(CancellationToken)) => this.EntitySet.AsQueryable().ToArrayAsync(cancellationToken);
    public Task<Dictionary<TKey, TEntity>> ToDictionaryAsync<TKey>(Func<TEntity, TKey> keySelector, CancellationToken cancellationToken = default(CancellationToken)) 
    where TKey : notnull
        => this.EntitySet.AsQueryable().ToDictionaryAsync(keySelector, cancellationToken);
    public Task<Dictionary<TKey, TEntity>> ToDictionaryAsync<TKey>(Func<TEntity, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken)) 
    where TKey : notnull
        => this.EntitySet.AsQueryable().ToDictionaryAsync(keySelector, comparer, cancellationToken);
    public Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TKey, TElement>(Func<TEntity, TKey> keySelector, Func<TEntity, TElement> elementSelector, CancellationToken cancellationToken = default(CancellationToken)) 
    where TKey : notnull 
        => this.EntitySet.AsQueryable().ToDictionaryAsync(keySelector, elementSelector, cancellationToken);
    public Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TKey, TElement>(Func<TEntity, TKey> keySelector, Func<TEntity, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken)) 
    where TKey : notnull 
        => this.EntitySet.AsQueryable().ToDictionaryAsync(keySelector, elementSelector, comparer, cancellationToken);
}

